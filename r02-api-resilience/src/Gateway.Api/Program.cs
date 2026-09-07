using System.Diagnostics;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var dependencyBaseUrl = builder.Configuration["Dependency:BaseUrl"]
    ?? throw new InvalidOperationException("Dependency:BaseUrl is not configured.");

builder.Services.AddHttpClient("dependency", client => client.BaseAddress = new Uri(dependencyBaseUrl));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/proxy/baseline", async (IHttpClientFactory clients, HttpResponse gatewayResponse, CancellationToken cancellationToken) =>
{
    var stopwatch = Stopwatch.StartNew();
    try
    {
        using var response = await clients.CreateClient("dependency").GetAsync("/dependency", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        gatewayResponse.Headers["X-Gateway-Elapsed-Milliseconds"] = stopwatch.ElapsedMilliseconds.ToString();
        return Results.Content(body, "application/json", statusCode: (int)response.StatusCode);
    }
    catch (HttpRequestException exception)
    {
        return Results.Json(new { error = exception.Message, elapsedMilliseconds = stopwatch.ElapsedMilliseconds }, statusCode: StatusCodes.Status502BadGateway);
    }
});

app.MapGet("/proxy/retry", async (IHttpClientFactory clients, CancellationToken cancellationToken) =>
{
    const int maxAttempts = 3;
    var stopwatch = Stopwatch.StartNew();
    HttpStatusCode? lastStatusCode = null;

    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        using var response = await clients.CreateClient("dependency").GetAsync("/dependency", cancellationToken);
        lastStatusCode = response.StatusCode;

        // E02 deliberately retries only this safe GET. Retrying writes is a separate problem.
        if ((int)response.StatusCode < StatusCodes.Status500InternalServerError)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            return Results.Json(new
            {
                attemptCount = attempt,
                dependencyStatus = (int)response.StatusCode,
                elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                response = JsonSerializer.Deserialize<JsonElement>(body)
            }, statusCode: (int)response.StatusCode);
        }

        if (attempt < maxAttempts)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
        }
    }

    return Results.Json(new
    {
        attemptCount = maxAttempts,
        dependencyStatus = (int?)lastStatusCode,
        elapsedMilliseconds = stopwatch.ElapsedMilliseconds,
        error = "Dependency remained unavailable after bounded retry."
    }, statusCode: StatusCodes.Status503ServiceUnavailable);
});

app.Run();

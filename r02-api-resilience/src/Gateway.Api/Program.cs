using System.Diagnostics;

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

app.Run();

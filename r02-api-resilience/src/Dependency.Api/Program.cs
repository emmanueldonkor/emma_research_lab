using Dependency.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DependencyState>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/control", (DependencyControlRequest request, DependencyState state) =>
{
    if (!Enum.TryParse<DependencyMode>(request.Mode, ignoreCase: true, out var mode) || request.DelayMilliseconds is < 0 or > 30_000)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["mode"] = ["Mode must be Healthy, Slow, or Unavailable; delayMilliseconds must be between 0 and 30000."]
        });
    }

    return Results.Ok(state.Configure(mode, request.DelayMilliseconds));
});

app.MapGet("/dependency", async (DependencyState state, CancellationToken cancellationToken) =>
{
    var requestNumber = state.RecordRequest();
    var snapshot = state.Snapshot();

    if (snapshot.Mode == DependencyMode.Slow)
    {
        await Task.Delay(snapshot.DelayMilliseconds, cancellationToken);
    }

    var response = new { requestNumber, mode = snapshot.Mode.ToString(), delayMilliseconds = snapshot.DelayMilliseconds };
    return snapshot.Mode == DependencyMode.Unavailable
        ? Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable)
        : Results.Ok(response);
});

app.Run();

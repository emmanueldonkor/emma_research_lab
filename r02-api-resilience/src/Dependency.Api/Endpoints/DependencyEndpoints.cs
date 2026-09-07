namespace Dependency.Api.Endpoints;

public static class DependencyEndpoints
{
    public static IEndpointRouteBuilder MapDependencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/control", Configure);
        endpoints.MapGet("/dependency", GetDependencyResponseAsync);
        return endpoints;
    }

    private static IResult Configure(DependencyControlRequest request, DependencyState state)
    {
        if (!Enum.TryParse<DependencyMode>(request.Mode, ignoreCase: true, out var mode) || request.DelayMilliseconds is < 0 or > 30_000)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["mode"] = ["Mode must be Healthy, Slow, or Unavailable; delayMilliseconds must be between 0 and 30000."]
            });
        }

        return Results.Ok(state.Configure(mode, request.DelayMilliseconds));
    }

    private static async Task<IResult> GetDependencyResponseAsync(DependencyState state, CancellationToken cancellationToken)
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
    }
}

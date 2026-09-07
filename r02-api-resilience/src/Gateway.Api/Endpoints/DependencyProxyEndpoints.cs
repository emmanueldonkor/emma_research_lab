using System.Text.Json;
using Gateway.Api.Services;

namespace Gateway.Api.Endpoints;

public static class DependencyProxyEndpoints
{
    public static IEndpointRouteBuilder MapDependencyProxyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/proxy/baseline", CallBaselineAsync);
        endpoints.MapGet("/proxy/retry", CallRetryAsync);
        endpoints.MapGet("/proxy/timeout", CallTimeoutAsync);
        return endpoints;
    }

    private static async Task<IResult> CallBaselineAsync(IDependencyGateway gateway, HttpResponse response, CancellationToken cancellationToken)
    {
        var result = await gateway.CallBaselineAsync(cancellationToken);
        response.Headers["X-Gateway-Elapsed-Milliseconds"] = result.ElapsedMilliseconds.ToString();
        return ToHttpResult(result, includeAttemptCount: false);
    }

    private static async Task<IResult> CallRetryAsync(IDependencyGateway gateway, CancellationToken cancellationToken) =>
        ToHttpResult(await gateway.CallWithBoundedRetryAsync(cancellationToken), includeAttemptCount: true);

    private static async Task<IResult> CallTimeoutAsync(IDependencyGateway gateway, CancellationToken cancellationToken) =>
        ToHttpResult(await gateway.CallWithTimeoutAsync(cancellationToken), includeAttemptCount: true);

    private static IResult ToHttpResult(DependencyCallResult result, bool includeAttemptCount)
    {
        if (result.Error is not null)
        {
            return Results.Json(new { attemptCount = result.AttemptCount, elapsedMilliseconds = result.ElapsedMilliseconds, error = result.Error }, statusCode: result.StatusCode);
        }

        if (!includeAttemptCount)
        {
            return Results.Content(result.Body, "application/json", statusCode: result.StatusCode);
        }

        return Results.Json(new
        {
            attemptCount = result.AttemptCount,
            dependencyStatus = result.StatusCode,
            elapsedMilliseconds = result.ElapsedMilliseconds,
            response = JsonSerializer.Deserialize<JsonElement>(result.Body!)
        }, statusCode: result.StatusCode);
    }
}

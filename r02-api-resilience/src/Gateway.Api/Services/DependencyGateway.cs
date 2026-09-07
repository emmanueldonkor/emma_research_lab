using System.Diagnostics;

namespace Gateway.Api.Services;

public sealed class DependencyGateway(IHttpClientFactory clients) : IDependencyGateway
{
    public Task<DependencyCallResult> CallBaselineAsync(CancellationToken cancellationToken) =>
        CallOnceAsync(cancellationToken);

    public async Task<DependencyCallResult> CallWithBoundedRetryAsync(CancellationToken cancellationToken)
    {
        const int maxAttempts = 3;
        var stopwatch = Stopwatch.StartNew();
        DependencyCallResult? lastResult = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var result = await CallOnceAsync(cancellationToken);
            lastResult = result with { AttemptCount = attempt, ElapsedMilliseconds = stopwatch.ElapsedMilliseconds };

            if (result.StatusCode < StatusCodes.Status500InternalServerError)
            {
                return lastResult;
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
            }
        }

        return lastResult!;
    }

    private async Task<DependencyCallResult> CallOnceAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var response = await clients.CreateClient("dependency").GetAsync("/dependency", cancellationToken);
            return new DependencyCallResult((int)response.StatusCode, 1, stopwatch.ElapsedMilliseconds, await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (HttpRequestException exception)
        {
            return new DependencyCallResult(StatusCodes.Status502BadGateway, 1, stopwatch.ElapsedMilliseconds, Error: exception.Message);
        }
    }
}

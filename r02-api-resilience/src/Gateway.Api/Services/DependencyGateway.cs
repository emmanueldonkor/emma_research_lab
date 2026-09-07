using System.Diagnostics;

namespace Gateway.Api.Services;

public sealed class DependencyGateway(IHttpClientFactory clients, IFailureCircuitBreaker circuitBreaker) : IDependencyGateway
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

    public async Task<DependencyCallResult> CallWithTimeoutAsync(CancellationToken cancellationToken)
    {
        const int timeoutMilliseconds = 250;
        var stopwatch = Stopwatch.StartNew();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(timeoutMilliseconds);

        try
        {
            using var response = await clients.CreateClient("dependency").GetAsync("/dependency", timeout.Token);
            return new DependencyCallResult((int)response.StatusCode, 1, stopwatch.ElapsedMilliseconds, await response.Content.ReadAsStringAsync(timeout.Token));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new DependencyCallResult(StatusCodes.Status504GatewayTimeout, 1, stopwatch.ElapsedMilliseconds, Error: $"Dependency attempt exceeded {timeoutMilliseconds} ms.");
        }
    }

    public async Task<CircuitCallResult> CallWithCircuitBreakerAsync(CancellationToken cancellationToken)
    {
        var permit = circuitBreaker.TryAcquire();
        if (!permit.Allowed)
        {
            return new CircuitCallResult(
                new DependencyCallResult(StatusCodes.Status503ServiceUnavailable, 0, 0, Error: "Circuit is open; dependency call was skipped."),
                permit.Snapshot,
                ShortCircuited: true);
        }

        var result = await CallOnceAsync(cancellationToken);
        var snapshot = circuitBreaker.RecordOutcome(result.StatusCode < StatusCodes.Status500InternalServerError);
        return new CircuitCallResult(result, snapshot, ShortCircuited: false);
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

namespace Gateway.Api.Services;

public sealed class FailureCircuitBreaker : IFailureCircuitBreaker
{
    private const int FailureThreshold = 3;
    private static readonly TimeSpan BreakDuration = TimeSpan.FromSeconds(1);
    private readonly object sync = new();
    private int consecutiveFailures;
    private DateTimeOffset? openedUntilUtc;

    public CircuitPermit TryAcquire()
    {
        lock (sync)
        {
            var now = DateTimeOffset.UtcNow;
            if (openedUntilUtc is { } openedUntil && openedUntil > now)
            {
                return new CircuitPermit(false, Snapshot(now));
            }

            if (openedUntilUtc is not null)
            {
                openedUntilUtc = null;
            }

            return new CircuitPermit(true, Snapshot(now));
        }
    }

    public CircuitSnapshot RecordOutcome(bool success)
    {
        lock (sync)
        {
            var now = DateTimeOffset.UtcNow;
            if (success)
            {
                consecutiveFailures = 0;
                openedUntilUtc = null;
                return Snapshot(now);
            }

            consecutiveFailures++;
            if (consecutiveFailures >= FailureThreshold)
            {
                openedUntilUtc = now.Add(BreakDuration);
            }

            return Snapshot(now);
        }
    }

    private CircuitSnapshot Snapshot(DateTimeOffset now)
    {
        var remaining = openedUntilUtc is { } openedUntil && openedUntil > now
            ? Math.Max(0, (long)(openedUntil - now).TotalMilliseconds)
            : 0;
        return new CircuitSnapshot(remaining > 0 ? "open" : "closed", consecutiveFailures, remaining);
    }
}

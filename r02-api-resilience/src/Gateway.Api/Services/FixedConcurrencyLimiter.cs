namespace Gateway.Api.Services;

public sealed class FixedConcurrencyLimiter : IConcurrencyLimiter
{
    private const int Limit = 2;
    private readonly SemaphoreSlim permits = new(Limit, Limit);
    private int activeRequests;

    public ConcurrencyPermit TryAcquire()
    {
        if (!permits.Wait(0))
        {
            return new ConcurrencyPermit(false, Snapshot());
        }

        var active = Interlocked.Increment(ref activeRequests);
        return new ConcurrencyPermit(true, new ConcurrencySnapshot(Limit, active));
    }

    public void Release()
    {
        Interlocked.Decrement(ref activeRequests);
        permits.Release();
    }

    private ConcurrencySnapshot Snapshot() =>
        new(Limit, Volatile.Read(ref activeRequests));
}

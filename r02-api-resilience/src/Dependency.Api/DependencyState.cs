namespace Dependency.Api;

public enum DependencyMode
{
    Healthy,
    Slow,
    Unavailable
}

public sealed class DependencyState
{
    private int requestCount;
    private int delayMilliseconds;
    private DependencyMode mode = DependencyMode.Healthy;

    public DependencySnapshot Configure(DependencyMode nextMode, int nextDelayMilliseconds)
    {
        mode = nextMode;
        delayMilliseconds = nextDelayMilliseconds;
        return Snapshot();
    }

    public int RecordRequest() => Interlocked.Increment(ref requestCount);

    public DependencySnapshot Snapshot() => new(mode, Volatile.Read(ref delayMilliseconds), Volatile.Read(ref requestCount));
}

public sealed record DependencySnapshot(DependencyMode Mode, int DelayMilliseconds, int RequestCount);

public sealed record DependencyControlRequest(string? Mode, int DelayMilliseconds = 0);

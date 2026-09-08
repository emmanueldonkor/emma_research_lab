namespace Dependency.Api;

public enum DependencyMode
{
    Healthy,
    Slow,
    Unavailable
}

public sealed class DependencyState
{
    private readonly object sync = new();
    private int requestCount;
    private int delayMilliseconds;
    private DependencyMode mode = DependencyMode.Healthy;

    public DependencySnapshot Configure(DependencyMode nextMode, int nextDelayMilliseconds)
    {
        lock (sync)
        {
            mode = nextMode;
            delayMilliseconds = nextDelayMilliseconds;
            return SnapshotUnsafe();
        }
    }

    public DependencySnapshot RecordRequestAndSnapshot()
    {
        lock (sync)
        {
            requestCount++;
            return SnapshotUnsafe();
        }
    }

    public DependencySnapshot Snapshot()
    {
        lock (sync)
        {
            return SnapshotUnsafe();
        }
    }

    private DependencySnapshot SnapshotUnsafe() => new(mode, delayMilliseconds, requestCount);
}

public sealed record DependencySnapshot(DependencyMode Mode, int DelayMilliseconds, int RequestCount);

public sealed record DependencyControlRequest(string? Mode, int DelayMilliseconds = 0);

namespace Gateway.Api.Services;

public sealed record CircuitSnapshot(string State, int ConsecutiveFailures, long RemainingBreakMilliseconds);

public sealed record CircuitPermit(bool Allowed, CircuitSnapshot Snapshot);

public sealed record CircuitCallResult(DependencyCallResult Call, CircuitSnapshot Snapshot, bool ShortCircuited);

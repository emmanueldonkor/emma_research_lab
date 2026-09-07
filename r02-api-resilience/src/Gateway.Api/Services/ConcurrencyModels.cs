namespace Gateway.Api.Services;

public sealed record ConcurrencySnapshot(int Limit, int ActiveRequests);

public sealed record ConcurrencyPermit(bool Allowed, ConcurrencySnapshot Snapshot);

public sealed record ConcurrencyCallResult(
    DependencyCallResult Call,
    ConcurrencySnapshot Snapshot,
    bool Rejected);

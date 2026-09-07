namespace Gateway.Api.Services;

public sealed record DependencyCallResult(
    int StatusCode,
    int AttemptCount,
    long ElapsedMilliseconds,
    string? Body = null,
    string? Error = null);

namespace Gateway.Api.Services;

public interface IConcurrencyLimiter
{
    ConcurrencyPermit TryAcquire();

    void Release();
}

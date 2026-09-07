namespace Gateway.Api.Services;

public interface IFailureCircuitBreaker
{
    CircuitPermit TryAcquire();

    CircuitSnapshot RecordOutcome(bool success);
}

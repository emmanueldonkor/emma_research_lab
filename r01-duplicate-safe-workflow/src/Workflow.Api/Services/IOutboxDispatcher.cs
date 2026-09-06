namespace Workflow.Api.Services;

public interface IOutboxDispatcher
{
    Task<OutboxDispatchResult> DispatchPendingAsync(bool simulateFailure, CancellationToken cancellationToken);
}

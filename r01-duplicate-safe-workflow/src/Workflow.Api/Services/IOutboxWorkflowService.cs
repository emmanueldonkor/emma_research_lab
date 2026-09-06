using Workflow.Api.Workflows;

namespace Workflow.Api.Services;

public interface IOutboxWorkflowService
{
    Task<OutboxWorkflowResult> CreateAsync(string name, bool simulateRollback, CancellationToken cancellationToken);

    Task<IReadOnlyList<OutboxMessage>> ListPendingAsync(CancellationToken cancellationToken);
}

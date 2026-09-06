using Workflow.Api.Workflows;

namespace Workflow.Api.Services;

public interface IWorkflowService
{
    Task<WorkflowCreationResult> CreateAsync(string name, string? idempotencyKey, CancellationToken cancellationToken);

    Task<IReadOnlyList<WorkflowRecord>> ListAsync(CancellationToken cancellationToken);
}

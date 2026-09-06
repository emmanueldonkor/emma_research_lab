namespace Workflow.Api.Workflows;

public sealed class IdempotencyRecord
{
    public required string Key { get; init; }

    public required string RequestFingerprint { get; init; }

    public Guid WorkflowId { get; init; }

    public required WorkflowRecord Workflow { get; init; }
}

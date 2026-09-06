namespace Workflow.Api.Workflows;

public sealed class WorkflowRecord
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }
}

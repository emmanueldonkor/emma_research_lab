namespace Workflow.Api.Workflows;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }

    public Guid WorkflowId { get; init; }

    public required WorkflowRecord Workflow { get; init; }

    public required string EventType { get; init; }

    public required string Payload { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? PublishedAtUtc { get; set; }
}

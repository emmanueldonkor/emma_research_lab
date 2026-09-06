namespace Workflow.Api.Workflows;

public sealed record CreateOutboxWorkflowRequest(string? Name, bool SimulateRollback = false);

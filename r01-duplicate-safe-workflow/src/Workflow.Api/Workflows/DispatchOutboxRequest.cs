namespace Workflow.Api.Workflows;

public sealed record DispatchOutboxRequest(bool SimulateFailure = false);

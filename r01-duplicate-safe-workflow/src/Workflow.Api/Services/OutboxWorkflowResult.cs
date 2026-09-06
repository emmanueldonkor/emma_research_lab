using Workflow.Api.Workflows;

namespace Workflow.Api.Services;

public sealed record OutboxWorkflowResult(bool Committed, WorkflowRecord? Workflow = null, OutboxMessage? Message = null);

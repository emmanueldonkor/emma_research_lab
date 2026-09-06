using Workflow.Api.Workflows;

namespace Workflow.Api.Services;

public enum WorkflowCreationOutcome
{
    Created,
    Replayed,
    KeyConflict
}

public sealed record WorkflowCreationResult(
    WorkflowCreationOutcome Outcome,
    WorkflowRecord? Workflow = null,
    string? Error = null);

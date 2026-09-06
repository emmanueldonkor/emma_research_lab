namespace Workflow.Api.Services;

public sealed record OutboxDispatchResult(int Attempted, int Published, int Failed);

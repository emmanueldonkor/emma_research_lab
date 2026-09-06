using Workflow.Api.Services;
using Workflow.Api.Workflows;

namespace Workflow.Api.Endpoints;

public static class WorkflowEndpoints
{
    public static IEndpointRouteBuilder MapWorkflowEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/workflows", CreateWorkflowAsync);
        endpoints.MapGet("/workflows", ListWorkflowsAsync);
        return endpoints;
    }

    private static async Task<IResult> CreateWorkflowAsync(CreateWorkflowRequest request, HttpRequest httpRequest, IWorkflowService workflows, CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["name"] = ["Name is required and must be 200 characters or fewer."] });
        }

        var idempotencyKey = httpRequest.Headers.TryGetValue("Idempotency-Key", out var values) ? values.ToString().Trim() : null;
        if (idempotencyKey is not null && (idempotencyKey.Length == 0 || idempotencyKey.Length > 128))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["Idempotency-Key"] = ["Idempotency-Key must be 128 characters or fewer when supplied."] });
        }

        var result = await workflows.CreateAsync(name, idempotencyKey, cancellationToken);
        return result.Outcome switch
        {
            WorkflowCreationOutcome.Created => Results.Created($"/workflows/{result.Workflow!.Id}", result.Workflow),
            WorkflowCreationOutcome.Replayed => Results.Ok(result.Workflow),
            WorkflowCreationOutcome.KeyConflict => Results.Conflict(new { error = result.Error }),
            _ => throw new InvalidOperationException("Unknown workflow creation outcome.")
        };
    }

    private static async Task<IResult> ListWorkflowsAsync(IWorkflowService workflows, CancellationToken cancellationToken) =>
        Results.Ok(await workflows.ListAsync(cancellationToken));
}

using Workflow.Api.Services;
using Workflow.Api.Workflows;

namespace Workflow.Api.Endpoints;

public static class OutboxWorkflowEndpoints
{
    public static IEndpointRouteBuilder MapOutboxWorkflowEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/outbox-workflows", CreateAsync);
        endpoints.MapGet("/outbox-messages", ListAsync);
        return endpoints;
    }

    private static async Task<IResult> CreateAsync(
        CreateOutboxWorkflowRequest request,
        IOutboxWorkflowService workflows,
        CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["name"] = ["Name is required and must be 200 characters or fewer."] });
        }

        var result = await workflows.CreateAsync(name, request.SimulateRollback, cancellationToken);
        return result.Committed
            ? Results.Created($"/outbox-workflows/{result.Workflow!.Id}", new { workflow = result.Workflow, outboxMessage = result.Message })
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<IResult> ListAsync(bool includePublished, IOutboxWorkflowService workflows, CancellationToken cancellationToken) =>
        Results.Ok(await workflows.ListAsync(includePublished, cancellationToken));
}

using Workflow.Api.Services;
using Workflow.Api.Workflows;

namespace Workflow.Api.Endpoints;

public static class OutboxDispatchEndpoints
{
    public static IEndpointRouteBuilder MapOutboxDispatchEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/outbox-dispatch", DispatchAsync);
        return endpoints;
    }

    private static async Task<IResult> DispatchAsync(DispatchOutboxRequest request, IOutboxDispatcher dispatcher, CancellationToken cancellationToken) =>
        Results.Ok(await dispatcher.DispatchPendingAsync(request.SimulateFailure, cancellationToken));
}

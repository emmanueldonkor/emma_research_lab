using Microsoft.EntityFrameworkCore;
using Workflow.Api.Persistence;

namespace Workflow.Api.Services;

public sealed class OutboxDispatcher(WorkflowDbContext database) : IOutboxDispatcher
{
    public async Task<OutboxDispatchResult> DispatchPendingAsync(bool simulateFailure, CancellationToken cancellationToken)
    {
        var messages = await database.OutboxMessages
            .Where(message => message.PublishedAtUtc == null)
            .OrderBy(message => message.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var published = 0;
        var failed = 0;
        foreach (var message in messages)
        {
            message.DeliveryAttempts++;
            if (simulateFailure)
            {
                message.LastError = "Simulated destination unavailable.";
                failed++;
                continue;
            }

            // E05 uses a controlled in-process delivery acknowledgement, not an external broker.
            message.PublishedAtUtc = DateTimeOffset.UtcNow;
            message.LastError = null;
            published++;
        }

        await database.SaveChangesAsync(cancellationToken);
        return new OutboxDispatchResult(messages.Count, published, failed);
    }
}

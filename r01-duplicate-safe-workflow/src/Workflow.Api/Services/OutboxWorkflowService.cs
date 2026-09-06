using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Workflow.Api.Persistence;
using Workflow.Api.Workflows;

namespace Workflow.Api.Services;

public sealed class OutboxWorkflowService(WorkflowDbContext database) : IOutboxWorkflowService
{
    public async Task<OutboxWorkflowResult> CreateAsync(string name, bool simulateRollback, CancellationToken cancellationToken)
    {
        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
        var workflow = new WorkflowRecord { Id = Guid.NewGuid(), Name = name, CreatedAtUtc = DateTimeOffset.UtcNow };
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            WorkflowId = workflow.Id,
            Workflow = workflow,
            EventType = "workflow.created.v1",
            Payload = JsonSerializer.Serialize(new { workflowId = workflow.Id, workflowName = workflow.Name }),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        database.OutboxMessages.Add(message);
        await database.SaveChangesAsync(cancellationToken);

        if (simulateRollback)
        {
            await transaction.RollbackAsync(cancellationToken);
            database.ChangeTracker.Clear();
            return new OutboxWorkflowResult(false);
        }

        await transaction.CommitAsync(cancellationToken);
        return new OutboxWorkflowResult(true, workflow, message);
    }

    public async Task<IReadOnlyList<OutboxMessage>> ListAsync(bool includePublished, CancellationToken cancellationToken)
    {
        var messages = database.OutboxMessages.AsNoTracking().OrderBy(message => message.CreatedAtUtc);
        return await (includePublished ? messages : messages.Where(message => message.PublishedAtUtc == null)).ToListAsync(cancellationToken);
    }
}

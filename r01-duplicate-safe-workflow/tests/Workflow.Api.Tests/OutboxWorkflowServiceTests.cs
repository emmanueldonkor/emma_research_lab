using Microsoft.EntityFrameworkCore;
using Workflow.Api.Services;

namespace Workflow.Api.Tests;

public sealed class OutboxWorkflowServiceTests
{
    [Fact]
    public async Task Rollback_removes_both_workflow_and_outbox_message()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = new OutboxWorkflowService(database.Context);

        var result = await service.CreateAsync("rollback-check", true, CancellationToken.None);

        Assert.False(result.Committed);
        Assert.Equal(0, await database.Context.WorkflowRecords.CountAsync());
        Assert.Equal(0, await database.Context.OutboxMessages.CountAsync());
    }

    [Fact]
    public async Task Failed_delivery_remains_pending_and_retry_marks_message_published()
    {
        await using var database = await TestDatabase.CreateAsync();
        var creator = new OutboxWorkflowService(database.Context);
        var dispatcher = new OutboxDispatcher(database.Context);
        var created = await creator.CreateAsync("retry-check", false, CancellationToken.None);

        var failure = await dispatcher.DispatchPendingAsync(true, CancellationToken.None);
        database.Context.ChangeTracker.Clear();
        var afterFailure = await database.Context.OutboxMessages.SingleAsync();
        var publishedAfterFailure = afterFailure.PublishedAtUtc;
        var attemptsAfterFailure = afterFailure.DeliveryAttempts;
        var errorAfterFailure = afterFailure.LastError;
        var retry = await dispatcher.DispatchPendingAsync(false, CancellationToken.None);
        database.Context.ChangeTracker.Clear();
        var afterRetry = await database.Context.OutboxMessages.SingleAsync();

        Assert.True(created.Committed);
        Assert.Equal(1, failure.Failed);
        Assert.Null(publishedAfterFailure);
        Assert.Equal(1, attemptsAfterFailure);
        Assert.NotNull(errorAfterFailure);
        Assert.Equal(1, retry.Published);
        Assert.NotNull(afterRetry.PublishedAtUtc);
        Assert.Equal(2, afterRetry.DeliveryAttempts);
        Assert.Null(afterRetry.LastError);
    }
}

using Microsoft.EntityFrameworkCore;
using Workflow.Api.Services;

namespace Workflow.Api.Tests;

public sealed class WorkflowServiceTests
{
    [Fact]
    public async Task Unkeyed_replays_create_distinct_workflows()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = new WorkflowService(database.Context);

        var first = await service.CreateAsync("invoice", null, CancellationToken.None);
        var second = await service.CreateAsync("invoice", null, CancellationToken.None);

        Assert.Equal(WorkflowCreationOutcome.Created, first.Outcome);
        Assert.Equal(WorkflowCreationOutcome.Created, second.Outcome);
        Assert.NotEqual(first.Workflow!.Id, second.Workflow!.Id);
        Assert.Equal(2, await database.Context.WorkflowRecords.CountAsync());
    }

    [Fact]
    public async Task Keyed_replay_returns_the_original_workflow()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = new WorkflowService(database.Context);

        var first = await service.CreateAsync("invoice", "same-key", CancellationToken.None);
        var replay = await service.CreateAsync("invoice", "same-key", CancellationToken.None);

        Assert.Equal(WorkflowCreationOutcome.Created, first.Outcome);
        Assert.Equal(WorkflowCreationOutcome.Replayed, replay.Outcome);
        Assert.Equal(first.Workflow!.Id, replay.Workflow!.Id);
        Assert.Equal(1, await database.Context.WorkflowRecords.CountAsync());
    }

    [Fact]
    public async Task Key_reused_with_a_different_body_is_rejected()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = new WorkflowService(database.Context);

        await service.CreateAsync("invoice", "same-key", CancellationToken.None);
        var conflict = await service.CreateAsync("other-invoice", "same-key", CancellationToken.None);

        Assert.Equal(WorkflowCreationOutcome.KeyConflict, conflict.Outcome);
        Assert.Equal(1, await database.Context.WorkflowRecords.CountAsync());
    }
}

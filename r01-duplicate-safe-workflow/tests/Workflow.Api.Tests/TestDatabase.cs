using Microsoft.EntityFrameworkCore;
using Workflow.Api.Persistence;

namespace Workflow.Api.Tests;

public sealed class TestDatabase : IAsyncDisposable
{
    private readonly WorkflowDbContext context;

    private TestDatabase(WorkflowDbContext context)
    {
        this.context = context;
    }

    public WorkflowDbContext Context => context;

    public static async Task<TestDatabase> CreateAsync()
    {
        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var context = new WorkflowDbContext(options);
        await context.Database.OpenConnectionAsync();
        await context.Database.EnsureCreatedAsync();
        return new TestDatabase(context);
    }

    public async ValueTask DisposeAsync()
    {
        await context.Database.CloseConnectionAsync();
        await context.DisposeAsync();
    }
}

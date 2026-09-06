using Microsoft.EntityFrameworkCore;
using Workflow.Api.Persistence;
using Workflow.Api.Workflows;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WorkflowDatabase")
    ?? throw new InvalidOperationException("Connection string 'WorkflowDatabase' is not configured.");

builder.Services.AddDbContext<WorkflowDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    await database.Database.EnsureCreatedAsync();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/workflows", async (
    CreateWorkflowRequest request,
    WorkflowDbContext database,
    CancellationToken cancellationToken) =>
{
    var name = request.Name?.Trim();

    if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = ["Name is required and must be 200 characters or fewer."]
        });
    }

    // E01 baseline: each HTTP request becomes a new record. No idempotency key is read.
    var workflow = new WorkflowRecord
    {
        Id = Guid.NewGuid(),
        Name = name,
        CreatedAtUtc = DateTimeOffset.UtcNow
    };

    database.WorkflowRecords.Add(workflow);
    await database.SaveChangesAsync(cancellationToken);

    return Results.Created($"/workflows/{workflow.Id}", workflow);
});

app.MapGet("/workflows", async (WorkflowDbContext database, CancellationToken cancellationToken) =>
{
    var workflows = await database.WorkflowRecords
        .AsNoTracking()
        .OrderBy(workflow => workflow.CreatedAtUtc)
        .ToListAsync(cancellationToken);

    return Results.Ok(workflows);
});

app.Run();

using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
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
    await database.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS idempotency_records (
            "Key" character varying(128) NOT NULL,
            "RequestFingerprint" character varying(64) NOT NULL,
            "WorkflowId" uuid NOT NULL,
            CONSTRAINT "PK_idempotency_records" PRIMARY KEY ("Key"),
            CONSTRAINT "FK_idempotency_records_workflow_records_WorkflowId"
                FOREIGN KEY ("WorkflowId") REFERENCES workflow_records ("Id") ON DELETE RESTRICT
        );
        """);
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/workflows", async (
    CreateWorkflowRequest request,
    HttpRequest httpRequest,
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

    if (!httpRequest.Headers.TryGetValue("Idempotency-Key", out var idempotencyKeyValues))
    {
        // E01 baseline: each HTTP request without a key becomes a new record.
        var baselineWorkflow = new WorkflowRecord
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        database.WorkflowRecords.Add(baselineWorkflow);
        await database.SaveChangesAsync(cancellationToken);
        return Results.Created($"/workflows/{baselineWorkflow.Id}", baselineWorkflow);
    }

    var idempotencyKey = idempotencyKeyValues.ToString().Trim();
    if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Length > 128)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["Idempotency-Key"] = ["Idempotency-Key is required and must be 128 characters or fewer when supplied."]
        });
    }

    var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(name)));
    var existing = await database.IdempotencyRecords
        .Include(record => record.Workflow)
        .SingleOrDefaultAsync(record => record.Key == idempotencyKey, cancellationToken);

    if (existing is not null)
    {
        return existing.RequestFingerprint == fingerprint
            ? Results.Ok(existing.Workflow)
            : Results.Conflict(new { error = "Idempotency-Key has already been used with a different request body." });
    }

    var workflow = new WorkflowRecord
    {
        Id = Guid.NewGuid(),
        Name = name,
        CreatedAtUtc = DateTimeOffset.UtcNow
    };

    database.IdempotencyRecords.Add(new IdempotencyRecord
    {
        Key = idempotencyKey,
        RequestFingerprint = fingerprint,
        WorkflowId = workflow.Id,
        Workflow = workflow
    });

    try
    {
        await database.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
        // A concurrent caller may have won the unique-key race. Load its result instead.
        database.ChangeTracker.Clear();
        existing = await database.IdempotencyRecords
            .Include(record => record.Workflow)
            .SingleOrDefaultAsync(record => record.Key == idempotencyKey, cancellationToken);

        if (existing is null)
        {
            throw;
        }

        return existing.RequestFingerprint == fingerprint
            ? Results.Ok(existing.Workflow)
            : Results.Conflict(new { error = "Idempotency-Key has already been used with a different request body." });
    }

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

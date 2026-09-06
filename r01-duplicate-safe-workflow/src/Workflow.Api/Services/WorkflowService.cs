using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Workflow.Api.Persistence;
using Workflow.Api.Workflows;

namespace Workflow.Api.Services;

public sealed class WorkflowService(WorkflowDbContext database) : IWorkflowService
{
    public async Task<WorkflowCreationResult> CreateAsync(string name, string? idempotencyKey, CancellationToken cancellationToken)
    {
        if (idempotencyKey is null)
        {
            var baselineWorkflow = CreateWorkflow(name);
            database.WorkflowRecords.Add(baselineWorkflow);
            await database.SaveChangesAsync(cancellationToken);
            return new WorkflowCreationResult(WorkflowCreationOutcome.Created, baselineWorkflow);
        }

        var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(name)));
        var existing = await FindIdempotencyRecordAsync(idempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return ExistingResult(existing, fingerprint);
        }

        var workflow = CreateWorkflow(name);
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
            return new WorkflowCreationResult(WorkflowCreationOutcome.Created, workflow);
        }
        catch (DbUpdateException)
        {
            database.ChangeTracker.Clear();
            existing = await FindIdempotencyRecordAsync(idempotencyKey, cancellationToken);
            if (existing is null)
            {
                throw;
            }

            return ExistingResult(existing, fingerprint);
        }
    }

    public async Task<IReadOnlyList<WorkflowRecord>> ListAsync(CancellationToken cancellationToken) =>
        await database.WorkflowRecords.AsNoTracking().OrderBy(workflow => workflow.CreatedAtUtc).ToListAsync(cancellationToken);

    private static WorkflowRecord CreateWorkflow(string name) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        CreatedAtUtc = DateTimeOffset.UtcNow
    };

    private Task<IdempotencyRecord?> FindIdempotencyRecordAsync(string key, CancellationToken cancellationToken) =>
        database.IdempotencyRecords.Include(record => record.Workflow)
            .SingleOrDefaultAsync(record => record.Key == key, cancellationToken);

    private static WorkflowCreationResult ExistingResult(IdempotencyRecord existing, string fingerprint) =>
        existing.RequestFingerprint == fingerprint
            ? new WorkflowCreationResult(WorkflowCreationOutcome.Replayed, existing.Workflow)
            : new WorkflowCreationResult(WorkflowCreationOutcome.KeyConflict, Error: "Idempotency-Key has already been used with a different request body.");
}

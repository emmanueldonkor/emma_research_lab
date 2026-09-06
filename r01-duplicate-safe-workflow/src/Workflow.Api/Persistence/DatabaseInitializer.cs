using Microsoft.EntityFrameworkCore;

namespace Workflow.Api.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        await database.Database.EnsureCreatedAsync(cancellationToken);
        await database.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS idempotency_records (
                "Key" character varying(128) NOT NULL,
                "RequestFingerprint" character varying(64) NOT NULL,
                "WorkflowId" uuid NOT NULL,
                CONSTRAINT "PK_idempotency_records" PRIMARY KEY ("Key"),
                CONSTRAINT "FK_idempotency_records_workflow_records_WorkflowId"
                    FOREIGN KEY ("WorkflowId") REFERENCES workflow_records ("Id") ON DELETE RESTRICT
            );
            """, cancellationToken);
    }
}

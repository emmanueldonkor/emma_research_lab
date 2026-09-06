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
        await database.Database.ExecuteSqlRawAsync("""
            ALTER TABLE outbox_messages ADD COLUMN IF NOT EXISTS "DeliveryAttempts" integer NOT NULL DEFAULT 0;
            ALTER TABLE outbox_messages ADD COLUMN IF NOT EXISTS "LastError" text NULL;
            """, cancellationToken);
        await database.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS outbox_messages (
                "Id" uuid NOT NULL,
                "WorkflowId" uuid NOT NULL,
                "EventType" character varying(100) NOT NULL,
                "Payload" text NOT NULL,
                "CreatedAtUtc" timestamp with time zone NOT NULL,
                "PublishedAtUtc" timestamp with time zone NULL,
                CONSTRAINT "PK_outbox_messages" PRIMARY KEY ("Id"),
                CONSTRAINT "FK_outbox_messages_workflow_records_WorkflowId"
                    FOREIGN KEY ("WorkflowId") REFERENCES workflow_records ("Id") ON DELETE RESTRICT
            );
            CREATE INDEX IF NOT EXISTS "IX_outbox_messages_PublishedAtUtc"
                ON outbox_messages ("PublishedAtUtc");
            """, cancellationToken);
    }
}

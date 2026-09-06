using Microsoft.EntityFrameworkCore;
using Workflow.Api.Workflows;

namespace Workflow.Api.Persistence;

public sealed class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : DbContext(options)
{
    public DbSet<WorkflowRecord> WorkflowRecords => Set<WorkflowRecord>();

    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var workflow = modelBuilder.Entity<WorkflowRecord>();
        workflow.ToTable("workflow_records");
        workflow.HasKey(record => record.Id);
        workflow.Property(record => record.Name).HasMaxLength(200).IsRequired();
        workflow.Property(record => record.CreatedAtUtc).IsRequired();

        var idempotency = modelBuilder.Entity<IdempotencyRecord>();
        idempotency.ToTable("idempotency_records");
        idempotency.HasKey(record => record.Key);
        idempotency.Property(record => record.Key).HasMaxLength(128);
        idempotency.Property(record => record.RequestFingerprint).HasMaxLength(64).IsRequired();
        idempotency.Property(record => record.WorkflowId).IsRequired();
        idempotency.HasOne(record => record.Workflow)
            .WithMany()
            .HasForeignKey(record => record.WorkflowId)
            .OnDelete(DeleteBehavior.Restrict);

        var outbox = modelBuilder.Entity<OutboxMessage>();
        outbox.ToTable("outbox_messages");
        outbox.HasKey(message => message.Id);
        outbox.Property(message => message.EventType).HasMaxLength(100).IsRequired();
        outbox.Property(message => message.Payload).IsRequired();
        outbox.Property(message => message.CreatedAtUtc).IsRequired();
        outbox.Property(message => message.DeliveryAttempts).IsRequired();
        outbox.Property(message => message.LastError);
        outbox.HasIndex(message => message.PublishedAtUtc);
        outbox.HasOne(message => message.Workflow)
            .WithMany()
            .HasForeignKey(message => message.WorkflowId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

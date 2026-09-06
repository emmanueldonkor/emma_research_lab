using Microsoft.EntityFrameworkCore;
using Workflow.Api.Workflows;

namespace Workflow.Api.Persistence;

public sealed class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : DbContext(options)
{
    public DbSet<WorkflowRecord> WorkflowRecords => Set<WorkflowRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var workflow = modelBuilder.Entity<WorkflowRecord>();
        workflow.ToTable("workflow_records");
        workflow.HasKey(record => record.Id);
        workflow.Property(record => record.Name).HasMaxLength(200).IsRequired();
        workflow.Property(record => record.CreatedAtUtc).IsRequired();
    }
}

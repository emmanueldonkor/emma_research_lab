using Microsoft.EntityFrameworkCore;
using Workflow.Api.Endpoints;
using Workflow.Api.Persistence;
using Workflow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WorkflowDatabase")
    ?? throw new InvalidOperationException("Connection string 'WorkflowDatabase' is not configured.");

builder.Services.AddDbContext<WorkflowDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IOutboxWorkflowService, OutboxWorkflowService>();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services);

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapWorkflowEndpoints();
app.MapOutboxWorkflowEndpoints();

app.Run();

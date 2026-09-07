using Gateway.Api.Endpoints;
using Gateway.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var dependencyBaseUrl = builder.Configuration["Dependency:BaseUrl"]
    ?? throw new InvalidOperationException("Dependency:BaseUrl is not configured.");

builder.Services.AddHttpClient("dependency", client => client.BaseAddress = new Uri(dependencyBaseUrl));
builder.Services.AddScoped<IDependencyGateway, DependencyGateway>();

var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapDependencyProxyEndpoints();

app.Run();

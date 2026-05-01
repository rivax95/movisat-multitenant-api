using Movisat.MultiTenantApi.Application;
using Movisat.MultiTenantApi.Api.Endpoints;
using Movisat.MultiTenantApi.Api.Middleware;
using Movisat.MultiTenantApi.Api.Options;
using Movisat.MultiTenantApi.Infrastructure;
using Movisat.MultiTenantApi.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.Configure<MultiTenantOptions>(
    builder.Configuration.GetSection(MultiTenantOptions.SectionName));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("Products") ?? "Data Source=products.db");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IProductDatabaseInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseMiddleware<RouteValidationMiddleware>();

app.MapHealthChecks("/health");
app.MapProductEndpoints();

app.Run();

/// <summary>
/// Public entry point used by WebApplicationFactory in integration tests.
/// </summary>
public partial class Program
{
}

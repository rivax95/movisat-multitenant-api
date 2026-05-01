using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Movisat.MultiTenantApi.Application.Abstractions;
using Movisat.MultiTenantApi.Infrastructure.Persistence;
using Movisat.MultiTenantApi.Infrastructure.Products;

namespace Movisat.MultiTenantApi.Infrastructure;

/// <summary>
/// Registers infrastructure services and adapters.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds EF Core, SQLite persistence and repository adapters to the container.
    /// </summary>
    /// <param name="services">Service collection used by the composition root.</param>
    /// <param name="connectionString">SQLite connection string used by EF Core.</param>
    /// <returns>The same service collection for fluent registration.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IProductReadRepository, EfProductReadRepository>();
        services.AddScoped<IProductDatabaseInitializer, ProductDatabaseInitializer>();

        return services;
    }
}

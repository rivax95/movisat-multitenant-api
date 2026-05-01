using Microsoft.Extensions.DependencyInjection;
using Movisat.MultiTenantApi.Application.Products;

namespace Movisat.MultiTenantApi.Application;

/// <summary>
/// Registers application-layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds product catalog use cases to the dependency injection container.
    /// </summary>
    /// <param name="services">Service collection used by the composition root.</param>
    /// <returns>The same service collection for fluent registration.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductCatalogService>();
        services.AddScoped<IProductQueryService>(provider =>
            provider.GetRequiredService<ProductCatalogService>());
        services.AddScoped<ICategoryLookup>(provider =>
            provider.GetRequiredService<ProductCatalogService>());

        return services;
    }
}

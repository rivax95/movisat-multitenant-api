using Microsoft.EntityFrameworkCore;
using Movisat.MultiTenantApi.Application.Abstractions;
using Movisat.MultiTenantApi.Application.Products;
using Movisat.MultiTenantApi.Domain.Products;
using Movisat.MultiTenantApi.Infrastructure.Persistence;

namespace Movisat.MultiTenantApi.Infrastructure.Products;

/// <summary>
/// EF Core implementation of product catalog read operations.
/// </summary>
public sealed class EfProductReadRepository(ProductsDbContext dbContext) : IProductReadRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Product>> ListAsync(
        ProductListQuery query,
        CancellationToken cancellationToken)
    {
        var products = dbContext.Products
            .AsNoTracking()
            .Where(product => product.ApiVersion == query.Version && product.Tenant == query.Tenant);

        if (!string.IsNullOrWhiteSpace(query.NormalizedCategory))
        {
            products = products.Where(product => product.Category == query.NormalizedCategory);
        }

        return await products
            .OrderBy(product => product.Category)
            .ThenBy(product => product.Sku)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> CategoryExistsAsync(
        int apiVersion,
        string tenant,
        string category,
        CancellationToken cancellationToken)
    {
        return dbContext.Products
            .AsNoTracking()
            .AnyAsync(
                product => product.ApiVersion == apiVersion
                    && product.Tenant == tenant
                    && product.Category == category,
                cancellationToken);
    }
}

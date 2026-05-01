using Movisat.MultiTenantApi.Application.Products;
using Movisat.MultiTenantApi.Domain.Products;

namespace Movisat.MultiTenantApi.Application.Abstractions;

/// <summary>
/// Defines the persistence operations required by product catalog use cases.
/// </summary>
public interface IProductReadRepository
{
    /// <summary>
    /// Returns products scoped by version, tenant and optional category.
    /// </summary>
    /// <param name="query">Catalog query with tenant isolation information.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>Products matching the query scope.</returns>
    Task<IReadOnlyCollection<Product>> ListAsync(
        ProductListQuery query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether a category exists inside a specific tenant/version context.
    /// </summary>
    /// <param name="apiVersion">API version requested by the client.</param>
    /// <param name="tenant">Tenant requested by the client.</param>
    /// <param name="category">Category requested by the client.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns><see langword="true"/> when at least one product exists in that category.</returns>
    Task<bool> CategoryExistsAsync(
        int apiVersion,
        string tenant,
        string category,
        CancellationToken cancellationToken);
}

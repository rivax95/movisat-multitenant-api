using Movisat.MultiTenantApi.Application.Abstractions;

namespace Movisat.MultiTenantApi.Application.Products;

/// <summary>
/// Coordinates product catalog read use cases without knowing how data is stored.
/// </summary>
public sealed class ProductCatalogService(IProductReadRepository productReadRepository)
    : IProductQueryService, ICategoryLookup
{
    /// <inheritdoc />
    public async Task<ProductListResult> ListAsync(
        ProductListQuery query,
        CancellationToken cancellationToken)
    {
        var products = await productReadRepository.ListAsync(query, cancellationToken);
        var items = products
            .Select(product => new ProductDto(
                product.Id,
                product.ApiVersion,
                product.Tenant,
                product.Category,
                product.Sku,
                product.Name,
                product.Price,
                product.UpdatedAt))
            .ToArray();

        return new ProductListResult(
            query.Version,
            query.Tenant,
            query.NormalizedCategory,
            items.Length,
            items);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
        int apiVersion,
        string tenant,
        string category,
        CancellationToken cancellationToken)
    {
        return productReadRepository.CategoryExistsAsync(
            apiVersion,
            tenant,
            category,
            cancellationToken);
    }
}

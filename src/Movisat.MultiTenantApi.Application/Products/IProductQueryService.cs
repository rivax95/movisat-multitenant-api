namespace Movisat.MultiTenantApi.Application.Products;

/// <summary>
/// Exposes the product catalog read use case.
/// </summary>
public interface IProductQueryService
{
    /// <summary>
    /// Lists products in the requested tenant/version scope.
    /// </summary>
    /// <param name="query">Query object produced from route and query string values.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Structured product list result.</returns>
    Task<ProductListResult> ListAsync(
        ProductListQuery query,
        CancellationToken cancellationToken);
}

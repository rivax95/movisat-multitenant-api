namespace Movisat.MultiTenantApi.Application.Products;

/// <summary>
/// Exposes category lookups for callers that only need validation, not product data.
/// </summary>
public interface ICategoryLookup
{
    /// <summary>
    /// Checks whether a category exists inside the requested tenant/version scope.
    /// </summary>
    /// <param name="apiVersion">Requested API version.</param>
    /// <param name="tenant">Requested tenant.</param>
    /// <param name="category">Requested category.</param>
    /// <param name="cancellationToken">Token used to cancel the lookup.</param>
    /// <returns><see langword="true"/> when the category exists in that scope.</returns>
    Task<bool> ExistsAsync(
        int apiVersion,
        string tenant,
        string category,
        CancellationToken cancellationToken);
}

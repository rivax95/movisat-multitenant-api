namespace Movisat.MultiTenantApi.Application.Products;

/// <summary>
/// Query object that carries the tenant/version scope extracted from HTTP.
/// </summary>
/// <param name="Version">Requested API version.</param>
/// <param name="Tenant">Requested tenant.</param>
/// <param name="Category">Optional product category filter.</param>
public sealed record ProductListQuery(int Version, string Tenant, string? Category)
{
    /// <summary>
    /// Category normalized to <see langword="null"/> when the client omitted it.
    /// </summary>
    public string? NormalizedCategory => string.IsNullOrWhiteSpace(Category)
        ? null
        : Category.Trim();
}

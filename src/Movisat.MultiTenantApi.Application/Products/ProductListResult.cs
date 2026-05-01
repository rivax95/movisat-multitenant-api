namespace Movisat.MultiTenantApi.Application.Products;

/// <summary>
/// Structured result returned by the product catalog read use case.
/// </summary>
/// <param name="Version">Requested API version.</param>
/// <param name="Tenant">Requested tenant.</param>
/// <param name="Category">Optional category filter after normalization.</param>
/// <param name="Count">Number of products returned.</param>
/// <param name="Items">Products in the requested scope.</param>
public sealed record ProductListResult(
    int Version,
    string Tenant,
    string? Category,
    int Count,
    IReadOnlyCollection<ProductDto> Items);

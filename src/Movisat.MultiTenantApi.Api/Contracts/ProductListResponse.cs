namespace Movisat.MultiTenantApi.Api.Contracts;

/// <summary>
/// JSON response returned by product list endpoints.
/// </summary>
/// <param name="Version">Requested API version.</param>
/// <param name="Tenant">Requested tenant.</param>
/// <param name="Category">Optional category filter.</param>
/// <param name="Count">Number of returned products.</param>
/// <param name="Items">Products visible in the requested scope.</param>
public sealed record ProductListResponse(
    int Version,
    string Tenant,
    string? Category,
    int Count,
    IReadOnlyCollection<ProductResponse> Items);

/// <summary>
/// JSON representation of a product.
/// </summary>
/// <param name="Id">Unique product identifier.</param>
/// <param name="Version">API version where the product is visible.</param>
/// <param name="Tenant">Tenant that owns the product.</param>
/// <param name="Category">Product category.</param>
/// <param name="Sku">Tenant-scoped stock keeping unit.</param>
/// <param name="Name">Human-readable product name.</param>
/// <param name="Price">Public product price.</param>
/// <param name="UpdatedAt">Timestamp of the last catalog update.</param>
public sealed record ProductResponse(
    Guid Id,
    int Version,
    string Tenant,
    string Category,
    string Sku,
    string Name,
    decimal Price,
    DateTimeOffset UpdatedAt);

namespace Movisat.MultiTenantApi.Application.Products;

/// <summary>
/// Product data returned by the application layer to presentation adapters.
/// </summary>
/// <param name="Id">Unique product identifier.</param>
/// <param name="Version">API version where the product is visible.</param>
/// <param name="Tenant">Tenant that owns the product.</param>
/// <param name="Category">Product category.</param>
/// <param name="Sku">Tenant-scoped stock keeping unit.</param>
/// <param name="Name">Human-readable product name.</param>
/// <param name="Price">Public product price.</param>
/// <param name="UpdatedAt">Timestamp of the last catalog update.</param>
public sealed record ProductDto(
    Guid Id,
    int Version,
    string Tenant,
    string Category,
    string Sku,
    string Name,
    decimal Price,
    DateTimeOffset UpdatedAt);

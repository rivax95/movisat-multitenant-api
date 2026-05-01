namespace Movisat.MultiTenantApi.Domain.Products;

/// <summary>
/// Represents a product that belongs to a single API version and tenant.
/// </summary>
public sealed class Product
{
    /// <summary>
    /// Initializes a product while preserving the invariants needed by the catalog.
    /// </summary>
    /// <param name="apiVersion">API version that exposes the product.</param>
    /// <param name="tenant">Tenant that owns the product.</param>
    /// <param name="category">Category used to group and filter the product.</param>
    /// <param name="sku">Tenant-scoped stock keeping unit.</param>
    /// <param name="name">Human-readable product name.</param>
    /// <param name="price">Public product price.</param>
    /// <param name="updatedAt">Last catalog update timestamp.</param>
    public Product(
        int apiVersion,
        string tenant,
        string category,
        string sku,
        string name,
        decimal price,
        DateTimeOffset updatedAt)
    {
        if (apiVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(apiVersion), "API version must be greater than zero.");
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        }

        ApiVersion = apiVersion;
        Tenant = RequireText(tenant, nameof(tenant));
        Category = RequireText(category, nameof(category));
        Sku = RequireText(sku, nameof(sku));
        Name = RequireText(name, nameof(name));
        Price = price;
        UpdatedAt = updatedAt;
    }

    private Product()
    {
    }

    /// <summary>
    /// Unique product identifier.
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Version of the public API where this product is visible.
    /// </summary>
    public int ApiVersion { get; private set; }

    /// <summary>
    /// Tenant that owns this row. Queries must always include this value.
    /// </summary>
    public string Tenant { get; private set; } = string.Empty;

    /// <summary>
    /// Product category used by the optional category filter.
    /// </summary>
    public string Category { get; private set; } = string.Empty;

    /// <summary>
    /// Tenant-scoped stock keeping unit.
    /// </summary>
    public string Sku { get; private set; } = string.Empty;

    /// <summary>
    /// Human-readable product name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Public product price.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Timestamp of the last catalog update.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty.", parameterName);
        }

        return value.Trim();
    }
}

namespace Movisat.MultiTenantApi.Infrastructure.Persistence;

/// <summary>
/// Ensures the product database exists and contains the baseline catalog.
/// </summary>
public sealed class ProductDatabaseInitializer(ProductsDbContext dbContext) : IProductDatabaseInitializer
{
    /// <inheritdoc />
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await ProductSeeder.SeedAsync(dbContext, cancellationToken);
    }
}

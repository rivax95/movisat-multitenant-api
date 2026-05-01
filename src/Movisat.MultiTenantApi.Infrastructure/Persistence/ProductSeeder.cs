using Microsoft.EntityFrameworkCore;
using Movisat.MultiTenantApi.Domain.Products;

namespace Movisat.MultiTenantApi.Infrastructure.Persistence;

/// <summary>
/// Inserts deterministic products for local execution and integration tests.
/// </summary>
public static class ProductSeeder
{
    /// <summary>
    /// Seeds products when the database is empty.
    /// </summary>
    /// <param name="dbContext">Product database context.</param>
    /// <param name="cancellationToken">Token used to cancel database writes.</param>
    public static async Task SeedAsync(
        ProductsDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var updatedAt = DateTimeOffset.Parse("2026-01-15T10:00:00Z");

        await dbContext.Products.AddRangeAsync(
            [
                new Product(
                    1,
                    "acme",
                    "navigation",
                    "ACME-NAV-100",
                    "Acme Route Tracker",
                    149.95m,
                    updatedAt),
                new Product(
                    1,
                    "acme",
                    "navigation",
                    "ACME-NAV-200",
                    "Acme Route Tracker Pro",
                    249.95m,
                    updatedAt),
                new Product(
                    1,
                    "acme",
                    "accessories",
                    "ACME-ACC-010",
                    "Acme Vehicle Mount",
                    29.95m,
                    updatedAt),
                new Product(
                    2,
                    "acme",
                    "navigation",
                    "ACME-NAV-300",
                    "Acme Route Tracker Cloud",
                    299.95m,
                    updatedAt),
                new Product(
                    1,
                    "beta",
                    "navigation",
                    "BETA-NAV-100",
                    "Beta Fleet Navigator",
                    129.50m,
                    updatedAt),
                new Product(
                    1,
                    "beta",
                    "diagnostics",
                    "BETA-DIA-050",
                    "Beta Engine Diagnostics",
                    89.00m,
                    updatedAt)
            ],
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

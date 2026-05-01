using Microsoft.EntityFrameworkCore;
using Movisat.MultiTenantApi.Domain.Products;

namespace Movisat.MultiTenantApi.Infrastructure.Persistence;

/// <summary>
/// EF Core database context that stores product catalog rows.
/// </summary>
public sealed class ProductsDbContext(DbContextOptions<ProductsDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Products available to the multi-tenant catalog.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);

            entity.Property(product => product.Tenant)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(product => product.Category)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(product => product.Sku)
                .HasMaxLength(64)
                .IsRequired();

            entity.Property(product => product.Name)
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(product => product.Price)
                .HasPrecision(10, 2);

            entity.HasIndex(product => new
            {
                product.ApiVersion,
                product.Tenant,
                product.Category
            });

            entity.HasIndex(product => new
            {
                product.ApiVersion,
                product.Tenant,
                product.Sku
            }).IsUnique();
        });
    }
}

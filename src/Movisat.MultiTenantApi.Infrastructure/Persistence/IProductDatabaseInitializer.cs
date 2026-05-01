namespace Movisat.MultiTenantApi.Infrastructure.Persistence;

/// <summary>
/// Initializes the product database used by the API composition root.
/// </summary>
public interface IProductDatabaseInitializer
{
    /// <summary>
    /// Creates the schema and inserts deterministic seed data when the database is empty.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel initialization.</param>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

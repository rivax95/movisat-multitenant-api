using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Movisat.MultiTenantApi.Api.Options;
using Movisat.MultiTenantApi.Infrastructure.Persistence;

namespace Movisat.MultiTenantApi.Tests.Infrastructure;

/// <summary>
/// Builds the API host for integration tests with an isolated in-memory SQLite database.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    /// <summary>
    /// Replaces the production database with a per-factory SQLite in-memory connection.
    /// </summary>
    /// <param name="builder">Web host builder created by <see cref="WebApplicationFactory{TEntryPoint}"/>.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ProductsDbContext>();
            services.RemoveAll<DbContextOptions<ProductsDbContext>>();

            _connection?.Dispose();
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<ProductsDbContext>(options =>
                options.UseSqlite(_connection));

            services.Configure<MultiTenantOptions>(options =>
            {
                options.SupportedVersions = [1, 2];
                options.BlockedTenants = ["sandbox", "test"];
            });
        });
    }

    /// <summary>
    /// Releases the SQLite connection kept open for the lifetime of the test host.
    /// </summary>
    /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}

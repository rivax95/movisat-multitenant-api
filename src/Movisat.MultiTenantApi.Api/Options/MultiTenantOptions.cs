namespace Movisat.MultiTenantApi.Api.Options;

/// <summary>
/// Configurable API routing rules for tenant and version validation.
/// </summary>
public sealed class MultiTenantOptions
{
    /// <summary>
    /// Configuration section name used by the application host.
    /// </summary>
    public const string SectionName = "MultiTenant";

    /// <summary>
    /// API versions accepted by route validation middleware.
    /// </summary>
    public int[] SupportedVersions { get; set; } = [1, 2];

    /// <summary>
    /// Tenants that are explicitly rejected before endpoint execution.
    /// </summary>
    public string[] BlockedTenants { get; set; } = ["sandbox", "test"];
}

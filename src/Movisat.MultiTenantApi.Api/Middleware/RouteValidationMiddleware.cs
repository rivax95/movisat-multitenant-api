using System.Globalization;
using Microsoft.Extensions.Options;
using Movisat.MultiTenantApi.Api.Options;

namespace Movisat.MultiTenantApi.Api.Middleware;

/// <summary>
/// Validates route-level multi-tenant constraints before endpoint execution.
/// </summary>
public sealed class RouteValidationMiddleware(
    RequestDelegate next,
    IOptions<MultiTenantOptions> options,
    ILogger<RouteValidationMiddleware> logger)
{
    /// <summary>
    /// Rejects unsupported API versions and blocked tenants.
    /// </summary>
    /// <param name="context">Current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint() is null)
        {
            await next(context);
            return;
        }

        var routeValues = context.Request.RouteValues;

        if (TryGetRouteVersion(routeValues, out var version)
            && !options.Value.SupportedVersions.Contains(version))
        {
            logger.LogWarning("Rejected unsupported API version {ApiVersion}", version);

            await WriteProblemAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Unsupported API version",
                $"Version '{version}' is not supported. Supported versions are: {string.Join(", ", options.Value.SupportedVersions)}.");

            return;
        }

        if (TryGetTenant(routeValues, out var tenant)
            && options.Value.BlockedTenants.Contains(tenant, StringComparer.OrdinalIgnoreCase))
        {
            logger.LogWarning("Rejected blocked tenant {Tenant}", tenant);

            await WriteProblemAsync(
                context,
                StatusCodes.Status403Forbidden,
                "Tenant is blocked",
                $"Tenant '{tenant}' is blocked for this API.");

            return;
        }

        await next(context);
    }

    private static bool TryGetRouteVersion(RouteValueDictionary routeValues, out int version)
    {
        version = 0;

        return routeValues.TryGetValue("version", out var rawVersion)
            && rawVersion is not null
            && int.TryParse(Convert.ToString(rawVersion, CultureInfo.InvariantCulture), out version);
    }

    private static bool TryGetTenant(RouteValueDictionary routeValues, out string tenant)
    {
        tenant = string.Empty;

        if (!routeValues.TryGetValue("tenant", out var rawTenant) || rawTenant is null)
        {
            return false;
        }

        tenant = Convert.ToString(rawTenant, CultureInfo.InvariantCulture) ?? string.Empty;
        return !string.IsNullOrWhiteSpace(tenant);
    }

    private static Task WriteProblemAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            instance: context.Request.Path)
            .ExecuteAsync(context);
    }
}

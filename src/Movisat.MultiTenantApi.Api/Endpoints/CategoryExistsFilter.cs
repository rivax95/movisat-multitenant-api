using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;
using Movisat.MultiTenantApi.Application.Products;

namespace Movisat.MultiTenantApi.Api.Endpoints;

/// <summary>
/// Validates the optional category query parameter before the endpoint handler runs.
/// </summary>
public sealed class CategoryExistsFilter(
    ICategoryLookup categoryLookup,
    ILogger<CategoryExistsFilter> logger) : IEndpointFilter
{
    /// <summary>
    /// Allows requests without a category and rejects unknown categories with 404.
    /// </summary>
    /// <param name="context">Endpoint invocation context.</param>
    /// <param name="next">Next filter or endpoint handler in the pipeline.</param>
    /// <returns>The endpoint result or a problem response.</returns>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var category = httpContext.Request.Query["category"].ToString();

        if (string.IsNullOrWhiteSpace(category))
        {
            return await next(context);
        }

        var normalizedCategory = category.Trim();
        var version = Convert.ToInt32(httpContext.Request.RouteValues["version"], CultureInfo.InvariantCulture);
        var tenant = Convert.ToString(httpContext.Request.RouteValues["tenant"], CultureInfo.InvariantCulture) ?? string.Empty;

        var categoryExists = await categoryLookup.ExistsAsync(
            version,
            tenant,
            normalizedCategory,
            httpContext.RequestAborted);

        if (categoryExists)
        {
            return await next(context);
        }

        logger.LogInformation(
            "Category {Category} was not found for tenant {Tenant} and version {ApiVersion}",
            normalizedCategory,
            tenant,
            version);

        return TypedResults.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Category not found",
            detail: $"Category '{normalizedCategory}' does not exist for tenant '{tenant}' in version '{version}'.",
            instance: httpContext.Request.Path);
    }
}

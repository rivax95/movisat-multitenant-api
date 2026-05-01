using System.Globalization;
using Movisat.MultiTenantApi.Application.Products;
using Movisat.MultiTenantApi.Api.Contracts;

namespace Movisat.MultiTenantApi.Api.Endpoints;

/// <summary>
/// Maps product catalog HTTP endpoints.
/// </summary>
public static class ProductEndpoints
{
    /// <summary>
    /// Registers all supported product route shapes.
    /// </summary>
    /// <param name="app">Endpoint route builder used by the ASP.NET Core host.</param>
    /// <returns>The same route builder for fluent configuration.</returns>
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        MapProductsRoute(app, "/api/v{version:int}/{tenant}/products");
        MapProductsRoute(app, "/api/{version:int}/{tenant}/products");

        return app;
    }

    private static void MapProductsRoute(IEndpointRouteBuilder app, string routePattern)
    {
        app.MapGet(routePattern, GetProductsAsync)
            .WithName(CreateRouteName(routePattern))
            .WithTags("Products")
            .WithOpenApi()
            .Produces<ProductListResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .AddEndpointFilter<CategoryExistsFilter>();
    }

    private static async Task<IResult> GetProductsAsync(
        int version,
        string tenant,
        string? category,
        IProductQueryService productQueryService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var products = await productQueryService.ListAsync(
            new ProductListQuery(version, tenant, category),
            cancellationToken);

        httpContext.Response.Headers["X-Tenant"] = tenant;
        httpContext.Response.Headers["X-Api-Version"] = version.ToString(CultureInfo.InvariantCulture);

        return Results.Ok(new ProductListResponse(
            products.Version,
            products.Tenant,
            products.Category,
            products.Count,
            products.Items.Select(ToResponse).ToArray()));
    }

    private static ProductResponse ToResponse(ProductDto product)
    {
        return new ProductResponse(
            product.Id,
            product.Version,
            product.Tenant,
            product.Category,
            product.Sku,
            product.Name,
            product.Price,
            product.UpdatedAt);
    }

    private static string CreateRouteName(string routePattern)
    {
        return routePattern.Contains("/v{", StringComparison.Ordinal)
            ? "GetProductsByVersionedTenant"
            : "GetProductsByTenant";
    }
}

using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Movisat.MultiTenantApi.Tests.Infrastructure;

namespace Movisat.MultiTenantApi.Tests;

/// <summary>
/// Verifies the product API by sending real HTTP requests through the test server.
/// </summary>
public sealed class ProductsApiTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory = new();
    private readonly HttpClient _client;

    /// <summary>
    /// Creates a fresh API host and HTTP client for each test.
    /// </summary>
    public ProductsApiTests()
    {
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    /// <summary>
    /// Validates the happy path for an existing category.
    /// </summary>
    [Fact]
    public async Task GetProducts_WithExistingCategory_ReturnsStructuredJson()
    {
        var response = await _client.GetAsync("/api/v1/acme/products?category=navigation");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        response.Headers.GetValues("X-Tenant").Should().ContainSingle().Which.Should().Be("acme");
        response.Headers.GetValues("X-Api-Version").Should().ContainSingle().Which.Should().Be("1");

        using var body = await ReadJsonAsync(response);
        var root = body.RootElement;
        root.GetProperty("version").GetInt32().Should().Be(1);
        root.GetProperty("tenant").GetString().Should().Be("acme");
        root.GetProperty("category").GetString().Should().Be("navigation");
        root.GetProperty("count").GetInt32().Should().Be(2);

        var items = root.GetProperty("items").EnumerateArray().ToArray();
        items.Should().HaveCount(2);
        items.Select(item => item.GetProperty("tenant").GetString()).Should().OnlyContain(tenant => tenant == "acme");
        items.Select(item => item.GetProperty("category").GetString()).Should().OnlyContain(category => category == "navigation");
    }

    /// <summary>
    /// Validates that unsupported route versions are rejected by middleware.
    /// </summary>
    [Fact]
    public async Task GetProducts_WithUnsupportedVersion_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/v3/acme/products?category=navigation");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        using var body = await ReadJsonAsync(response);
        body.RootElement.GetProperty("title").GetString().Should().Be("Unsupported API version");
        body.RootElement.GetProperty("status").GetInt32().Should().Be(400);
    }

    /// <summary>
    /// Validates that blocked tenants are rejected before the endpoint handler executes.
    /// </summary>
    [Theory]
    [InlineData("test")]
    [InlineData("sandbox")]
    public async Task GetProducts_WithBlockedTenant_ReturnsForbidden(string tenant)
    {
        var response = await _client.GetAsync($"/api/v1/{tenant}/products");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        using var body = await ReadJsonAsync(response);
        body.RootElement.GetProperty("title").GetString().Should().Be("Tenant is blocked");
        body.RootElement.GetProperty("status").GetInt32().Should().Be(403);
    }

    /// <summary>
    /// Validates that an unknown category returns 404 inside the tenant/version scope.
    /// </summary>
    [Fact]
    public async Task GetProducts_WithUnknownCategory_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/acme/products?category=unknown");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        using var body = await ReadJsonAsync(response);
        body.RootElement.GetProperty("title").GetString().Should().Be("Category not found");
        body.RootElement.GetProperty("status").GetInt32().Should().Be(404);
    }

    /// <summary>
    /// Validates the design decision that omitting category returns all products in scope.
    /// </summary>
    [Fact]
    public async Task GetProducts_WithoutCategory_ReturnsAllProductsForTenantAndVersion()
    {
        var response = await _client.GetAsync("/api/v1/acme/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await ReadJsonAsync(response);
        var root = body.RootElement;
        root.GetProperty("category").ValueKind.Should().Be(JsonValueKind.Null);
        root.GetProperty("count").GetInt32().Should().Be(3);

        var items = root.GetProperty("items").EnumerateArray().ToArray();
        items.Select(item => item.GetProperty("tenant").GetString()).Should().OnlyContain(tenant => tenant == "acme");
        items.Select(item => item.GetProperty("version").GetInt32()).Should().OnlyContain(version => version == 1);
        items.Select(item => item.GetProperty("category").GetString())
            .Should()
            .BeEquivalentTo(["accessories", "navigation", "navigation"]);
    }

    /// <summary>
    /// Verifies that products from one tenant are never returned to another tenant.
    /// </summary>
    [Fact]
    public async Task GetProducts_DoesNotLeakProductsBetweenTenants()
    {
        var acmeResponse = await _client.GetAsync("/api/v1/acme/products?category=navigation");
        var betaResponse = await _client.GetAsync("/api/v1/beta/products?category=navigation");

        acmeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        betaResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var acmeBody = await ReadJsonAsync(acmeResponse);
        using var betaBody = await ReadJsonAsync(betaResponse);

        var acmeSkus = acmeBody.RootElement.GetProperty("items")
            .EnumerateArray()
            .Select(item => item.GetProperty("sku").GetString())
            .ToArray();

        var betaSkus = betaBody.RootElement.GetProperty("items")
            .EnumerateArray()
            .Select(item => item.GetProperty("sku").GetString())
            .ToArray();

        acmeSkus.Should().OnlyContain(sku => sku != null && sku.StartsWith("ACME-", StringComparison.Ordinal));
        betaSkus.Should().OnlyContain(sku => sku != null && sku.StartsWith("BETA-", StringComparison.Ordinal));
        acmeSkus.Should().NotIntersectWith(betaSkus);
    }

    /// <summary>
    /// Verifies that products from one API version are not returned by another version.
    /// </summary>
    [Fact]
    public async Task GetProducts_DoesNotLeakProductsBetweenVersions()
    {
        var response = await _client.GetAsync("/api/v2/acme/products?category=navigation");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await ReadJsonAsync(response);
        var items = body.RootElement.GetProperty("items").EnumerateArray().ToArray();

        items.Should().ContainSingle();
        items[0].GetProperty("version").GetInt32().Should().Be(2);
        items[0].GetProperty("sku").GetString().Should().Be("ACME-NAV-300");
    }

    /// <summary>
    /// Validates the alternate route shape supported for the ambiguous challenge wording.
    /// </summary>
    [Fact]
    public async Task GetProducts_AlsoSupportsRouteWithoutVersionPrefix()
    {
        var response = await _client.GetAsync("/api/1/acme/products?category=navigation");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var body = await ReadJsonAsync(response);
        body.RootElement.GetProperty("count").GetInt32().Should().Be(2);
    }

    /// <summary>
    /// Disposes the HTTP client and the isolated test host.
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(stream);
    }
}

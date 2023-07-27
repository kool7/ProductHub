using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ProductHub.Domain.Product;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ProductHub.IntegrationTests;

public class ProductControllerIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _appFactory;

    public ProductControllerIntegrationTests()
    {
        _appFactory = new WebApplicationFactory<Program>();
        _httpClient = _appFactory.CreateClient();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetProductsAsync_Success_EndpointReturnsOkResult()
    {
        // Arrange
        var expectedProducts = _fixture.CreateMany<Product>();

        // Act
        var response = await _httpClient.GetAsync("https://localhost:44363/api/products");

        // Assert
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        products.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetProductAsync_ValidId_EndpointReturnsOkResult()
    {
        // Arrange
        var productId = _fixture.Create<string>();

        // Act
        var response = await _httpClient.GetAsync($"https://localhost:44363/api/products/{productId}");

        // Assert
        var product = await response.Content.ReadFromJsonAsync<Product>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        product?.Id.Should().Be(productId);
    }

    [Fact]
    public async Task GetProductAsync_InValidId_EndpointReturnsNotFoundResult()
    {
        // Arrange
        var productId = _fixture.Create<string>();

        // Act
        var response = await _httpClient.GetAsync($"https://localhost:44363/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProductAsync_Success_EndpointReturnsCreatedAtRouteResult()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(product),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PostAsync("https://localhost:44363/api/products", httpContent);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<Product>();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task UpdateProductAsync_Success_EndpointReturnsNoContentResult()
    {
        // Arrange
        var productId = _fixture.Create<string>();
        var product = _fixture.Create<Product>();
        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(product),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _httpClient.PutAsync($"https://localhost:44363/api/products/{productId}", httpContent);

        // Assert
        var productResponse = await _httpClient.GetAsync($"https://localhost:44363/api/products/{productId}");
        var updatedProduct = await productResponse.Content.ReadFromJsonAsync<Product>();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        updatedProduct?.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task DeleteProductAsync_Success_EndpointReturnsNoContentResult()
    {
        // Arrange
        var productId = _fixture.Create<string>();

        // Act
        var response = await _httpClient.DeleteAsync($"https://localhost:44363/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getUserResponse = await _httpClient.GetAsync($"https://localhost:44363/api/products/{productId}");
        getUserResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

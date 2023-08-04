using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using ProductHub.Application.Common;
using ProductHub.Api.Services;
using ProductHub.Application.Contracts.Products;
using ProductHub.Domain.Products;
using ProductHub.Infrastructure.Repositories;
using ProductHub.Infrastructure.Utility;
using ProductHub.IntegrationTests.Utility;
using System.Net;
using System.Net.Http.Json;

namespace ProductHub.IntegrationTests;

public class ProductControllerIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Fixture _fixture;
    private readonly HttpClient _httpClient;
    private readonly WebApplicationFactory<Program> _appFactory;

    public ProductControllerIntegrationTests()
    {
        _appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Testing.json");
                });

                builder.ConfigureServices(services =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    var mongoDbSettings = configuration.GetSection("MongoDB").Get<MongoDBSettings>();
                    services.AddSingleton<ProductDbContext>();
                    services.AddScoped<IProductsService, ProductsService>();
                    services.AddScoped<IProductRepository, ProductRepository>();
                });
            });

        _httpClient = _appFactory.CreateClient();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetProductsAsync_WithDefaultParameters_EndpointReturnsOkResult()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var expectedProducts = MongoDbConfig.seedProducts;

        // Act
        var response = await _httpClient.GetAsync(ProductHubConstants.GetProductsAsync);

        // Assert
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        products.Should().BeEquivalentTo(expectedProducts);
    }

    [Theory]
    [InlineData(1, 5, "a", "asc")]
    [InlineData(2, 10, "b", "desc")]
    public async Task GetProductsAsync_WithValidQueryParameters_ShouldReturnOkResultsFilteredProducts(int pageNumber, int pageSize, string searchTerm, string sort)
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var productsSeed = MongoDbConfig.seedProducts;
        var filteredProducts = productsSeed
                .Where(p =>
                    (p.Description.Contains(searchTerm) || p.Name.Contains(searchTerm)));

        var expectedProducts = filteredProducts;

        if (sort == "asc")
        {
            expectedProducts = expectedProducts.OrderBy(product => product.Units);
        }
        else if (sort == "desc")
        {
            expectedProducts = expectedProducts.OrderByDescending(product => product.Units);
        }

        expectedProducts = expectedProducts
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var queryString = MongoDbConfig.CreateQueryString(new
        {
            pageNumber,
            pageSize,
            searchTerm,
            sort
        });

        // Act
        var response = await _httpClient.GetAsync(ProductHubConstants.GetProductsAsync + queryString);

        // Assert
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        products.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task GetProductAsync_ValidId_EndpointReturnsOkResult()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var expectedProduct = MongoDbConfig.seedProducts.FirstOrDefault();

        // Act
        var response = await _httpClient.GetAsync(string.Format(ProductHubConstants.GetProductAsync, expectedProduct.Id));

        // Assert
        var product = await response.Content.ReadFromJsonAsync<Product>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        product.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact]
    public async Task GetProductAsync_InValidId_EndpointReturnsNotFoundResult()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var productId = ObjectId.GenerateNewId().ToString();

        // Act
        var response = await _httpClient.GetAsync(string.Format(ProductHubConstants.GetProductAsync, productId));

        // Assert
        var product = await response.Content.ReadFromJsonAsync<Product>();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProductAsync_Success_EndpointReturnsCreatedAtRouteResult()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var expectedProduct = _fixture.Create<CreateProductDto>();

        // Act
        var response = await _httpClient.PostAsJsonAsync(ProductHubConstants.CreateProductAsync, expectedProduct);

        // Assert
        var product = await response.Content.ReadFromJsonAsync<Product>();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        product.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact]
    public async Task CreateProductAsync_InValidProduct_EndpointReturnsBadRequest()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var existingProduct = MongoDbConfig.seedProducts.FirstOrDefault();

        // Act
        var response = await _httpClient.PostAsJsonAsync(ProductHubConstants.CreateProductAsync, existingProduct);

        // Assert
        var product = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        product.Should().Contain(string.Format(ProductHubResponse.ProductAlreadyExists, existingProduct.Name));
    }

    [Fact]
    public async Task UpdateProductAsync_Success_EndpointReturnsNoContentResult()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var productToUpdate = MongoDbConfig.seedProducts.FirstOrDefault();
        var updatedProductDto = _fixture.Create<UpdateProductDto>();

        // Act
        var response = await _httpClient.PutAsJsonAsync(string.Format(ProductHubConstants.UpdateProductAsync, productToUpdate.Id), updatedProductDto);

        // Assert
        var updatedProduct = await response.Content.ReadFromJsonAsync<Product>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedProduct.Should().BeEquivalentTo(updatedProductDto);
    }

    [Fact]
    public async Task UpdateProductAsync_ValidIdNonExistentProduct_EndpointReturnsNotFound()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var nonExistentProductId = ObjectId.GenerateNewId().ToString();
        var updatedProductDto = _fixture.Create<UpdateProductDto>();

        // Act
        var response = await _httpClient.PutAsJsonAsync(string.Format(ProductHubConstants.UpdateProductAsync, nonExistentProductId), updatedProductDto);

        // Assert
        var productResponse = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        productResponse.Should().Contain(ProductHubResponse.ProductNotFound);
    }

    [Fact]
    public async Task UpdateProductAsync_InvalidId_EndpointReturnsBadRequest()
    {
        // Arrange
        var invalidId = _fixture.Create<string>();
        var updateProductDto = _fixture.Create<UpdateProductDto>();

        // Act
        var response = await _httpClient.PutAsJsonAsync(string.Format(ProductHubConstants.UpdateProductAsync, invalidId), updateProductDto);

        // Assert
        var productResponse = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        productResponse.Should().Contain(ProductHubResponse.InvalidProductId);
    }

    [Fact]
    public async Task DeleteProductAsync_ValidIdExistentProduct_EndpointReturnsNoContentResult()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var productToRemove = MongoDbConfig.seedProducts.FirstOrDefault();

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(ProductHubConstants.DeleteProductAsync, productToRemove.Id));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getUserResponse = await _httpClient.GetAsync(string.Format(ProductHubConstants.GetProductAsync, productToRemove.Id));
        getUserResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProductAsync_ValidIdNonExistentProduct_EndpointReturnsNotFound()
    {
        // Arrange
        MongoDbConfig.InitializeDbForTests(_appFactory);
        var nonExistentProductId = ObjectId.GenerateNewId().ToString();

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(ProductHubConstants.DeleteProductAsync, nonExistentProductId));

        // Assert
        var productResponse = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        productResponse.Should().Contain(ProductHubResponse.ProductNotFound);
    }

    [Fact]
    public async Task DeleteProductAsync_InvalidId_EndpointReturnsBadRequest()
    {
        // Arrange
        var invalidId = _fixture.Create<string>();

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(ProductHubConstants.DeleteProductAsync, invalidId));

        // Assert
        var productResponse = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        productResponse.Should().Contain(ProductHubResponse.InvalidProductId);
    }
}

using Microsoft.Extensions.Logging;
using ProductHub.Api.Controllers;
using Moq;
using ProductHub.Api.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AutoFixture;
using ProductHub.Api.Models;
using FluentAssertions;
using ProductHub.Api;

namespace ProductHub.UnitTests.Controller;

public class ProductTestController
{
    private readonly Fixture _fixture;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly Mock<IProductsService> _mockProductsService;
    private readonly IMapper _mockMapper;
    private readonly ProductsController _sutProductsController;

    public ProductTestController()
    {
        _fixture = new Fixture();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _mockProductsService = new Mock<IProductsService>();

        if (_mockMapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ProductHubAutoMapperProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mockMapper = mapper;
        }

        _sutProductsController = new ProductsController(_mockLogger.Object,
            _mockProductsService.Object,
            _mockMapper);
    }

    [Fact]
    public async Task GetProductsAsync_ListOfProducts_ReturnsOkStatus()
    {
        // Arrange
        var products = _fixture.CreateMany<Product>();
        _mockProductsService.Setup(service => service.GetProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _sutProductsController.GetProductsAsync();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        result.Value?.Count().Should().Be(products.Count());
    }

    [Fact]
    public async Task GetProductAsync_ValidProductId_ReturnsProduct()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        _mockProductsService
            .Setup(service => service.GetProductAsync(product.Id!))
            .ReturnsAsync(product);

        // Act
        var result = await _sutProductsController.GetProductAsync(product.Id!);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetProductAsync_InValidProductId_ReturnsProduct()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        _mockProductsService
            .Setup(service => service.GetProductAsync(product.Id!))
            .ReturnsAsync((Product)null!);

        // Act
        var result = await _sutProductsController.GetProductAsync(product.Id!);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateProductAsync_ReturnsCreatedAtRoute()
    {
        // Arrange
        var createProductDto = _fixture.Create<CreateProductDto>();
        var product = _fixture.Create<Product>();

        _mockProductsService
            .Setup(service => service.AddProductAsync(product))
            .ReturnsAsync(product);

        // Act
        var result = await _sutProductsController.CreateProductAsync(createProductDto);

        // Assert
        result.Should().BeOfType<CreatedAtRouteResult>();
        result.Should().BeEquivalentTo(product);

        var createdAtRouteResult = (CreatedAtRouteResult)result;
        createdAtRouteResult?.StatusCode.Should().Be(201);
        createdAtRouteResult?.RouteName.Should().BeEquivalentTo("GetProduct");
        createdAtRouteResult?.RouteValues!["Id"].Should().BeEquivalentTo(product.Id);
    }

    [Fact]
    public async Task DeleteUserAsync_ValidUserId_RemovesUser()
    {
        // Arrange
        var productToRemove = _fixture.Create<Product>();
        _mockProductsService
            .Setup(service => service.DeleteProductAsync(productToRemove.Id!));

        // Act
        var result = await _sutProductsController.DeleteProductAsync(productToRemove.Id!);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockProductsService.Verify(service => service.DeleteProductAsync(productToRemove.Id), Times.Once);
    }
}
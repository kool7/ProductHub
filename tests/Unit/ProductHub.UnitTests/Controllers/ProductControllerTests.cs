using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using ProductHub.Api;
using ProductHub.Api.Controllers;
using ProductHub.Application.Contracts.Products;
using ProductHub.Domain.Products;
using ProductHub.UnitTests.Utility;
using static ProductHub.UnitTests.Utility.ProductHubFixtures;

namespace ProductHub.UnitTests.Controller
{
    public class ProductControllerTests
    {
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly Mock<IProductsService> _mockProductsService;
        private readonly IMapper _mockMapper;
        private readonly ProductsController _sutProductsController;
        private readonly Fixture _fixture;

        public ProductControllerTests()
        {
            _fixture = new Fixture();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _mockProductsService = new Mock<IProductsService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductHubAutoMapperProfile>();
            });

            _mockMapper = configuration.CreateMapper();

            _sutProductsController = new ProductsController(_mockLogger.Object, _mockProductsService.Object, _mockMapper);
        }

        [Fact]
        public async Task GetProductsAsync_DefaultParams_ShouldReturnOkResultWithProducts()
        {
            // Arrange
            var products = _fixture.Build<Product>()
                                    .Without(p => p.Id)
                                    .Do(x => x.Id = ObjectId.GenerateNewId().ToString())
                                    .CreateMany(50)
                                    .ToList();
            var (pageNumber, pageSize, searchTerm, sort) = ProductHubFixtures.GeneratePaginationData(PaginationScenario.DefaultParameters);
            _mockProductsService.Setup(x => x.GetProductsAsync(pageNumber, pageSize, searchTerm, sort))
                .ReturnsAsync(products);

            // Act
            var result = await _sutProductsController.GetProductsAsync(pageNumber, pageSize, searchTerm, sort);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(products);
            _mockProductsService.Verify(service => service.GetProductsAsync(pageNumber, pageSize, searchTerm, sort), Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_ValidParams_ShouldReturnOkResultWithProducts()
        {
            // Arrange
            var products = _fixture.Build<Product>()
                                    .Without(p => p.Id)
                                    .Do(x => x.Id = ObjectId.GenerateNewId().ToString())
                                    .CreateMany(50)
                                    .ToList();
            var (pageNumber, pageSize, searchTerm, sort) = ProductHubFixtures.GeneratePaginationData(PaginationScenario.AllValid);
            _mockProductsService.Setup(x => x.GetProductsAsync(pageNumber, pageSize, searchTerm, sort))
                .ReturnsAsync(products);

            // Act
            var result = await _sutProductsController.GetProductsAsync(pageNumber, pageSize, searchTerm, sort);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(products);
            _mockProductsService.Verify(service => service.GetProductsAsync(pageNumber, pageSize, searchTerm, sort), Times.Once);
        }

        [Fact]
        public async Task GetProductAsync_ValidId_ShouldReturnOkResultWithProduct()
        {
            // Arrange
            var product = _fixture.Create<Product>();
            _mockProductsService.Setup(x => x.GetProductAsync(product.Id))
                .ReturnsAsync(product);

            // Act
            var result = await _sutProductsController.GetProductAsync(product.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(product);
            _mockProductsService.Verify(service => service.GetProductAsync(product.Id), Times.Once);
        }

        [Fact]
        public async Task GetProductAsync_InValidId_ShouldReturnNotFoundResult()
        {
            // Arrange
            var product = _fixture.Create<Product>();
            _mockProductsService.Setup(x => x.GetProductAsync(product.Id))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _sutProductsController.GetProductAsync(product.Id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockProductsService.Verify(service => service.GetProductAsync(product.Id), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_ShouldReturnCreatedAtRouteResult()
        {
            // Arrange
            var createProductDto = _fixture.Create<CreateProductDto>();
            var product = _fixture.Create<Product>();
            _mockProductsService.Setup(x => x.AddProductAsync(product))
                .ReturnsAsync(product);

            // Act
            var result = await _sutProductsController.CreateProductAsync(createProductDto);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>().Which.RouteName.Should().Be("GetProduct");
            result.Should().BeOfType<CreatedAtRouteResult>().Which.Value.Should().Be(product);
        }

        [Fact]
        public async Task UpdateProductAsync_ValidProduct_ShouldReturnOkResultWithUpdatedProduct()
        {
            // Arrange
            var updateProductDto = _fixture.Create<UpdateProductDto>();
            var product = _fixture.Build<Product>()
                                  .Without(p => p.Id)
                                  .Do(x => x.Id = ObjectId.GenerateNewId().ToString())
                                  .With(x => x.Name, updateProductDto.Name)
                                  .With(x => x.Description, updateProductDto.Description)
                                  .With(x => x.Price, updateProductDto.Price)
                                  .With(x => x.Units, updateProductDto.Units)
                                  .Create();
            var productId = product.Id;

            _mockProductsService.Setup(x => x.UpdateProductAsync(productId, product))
                .ReturnsAsync(product);

            // Act
            var result = await _sutProductsController.UpdateProductAsync(productId, updateProductDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task DeleteProductAsync_ValidId_ShouldReturnNoContentResult()
        {
            // Arrange
            var Id = ObjectId.GenerateNewId().ToString();
            _mockProductsService.Setup(x => x.DeleteProductAsync(Id));

            // Act
            var result = await _sutProductsController.DeleteProductAsync(Id);

            // Assert
            _mockProductsService.Verify(service => service.DeleteProductAsync(Id), Times.Once);
            result.Should().BeOfType<NoContentResult>();
        }
    }
}

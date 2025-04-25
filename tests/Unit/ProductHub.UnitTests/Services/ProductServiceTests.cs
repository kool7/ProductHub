using AutoFixture;
using FluentAssertions;
using FluentValidation;
using MongoDB.Bson;
using Moq;
using ProductHub.Api.Services;
using ProductHub.Application.Common;
using ProductHub.Domain.Common;
using ProductHub.Domain.Exceptions;
using ProductHub.Domain.Products;
using ProductHub.Domain.Validation;
using ProductHub.UnitTests.Utility;
using static ProductHub.UnitTests.Utility.ProductHubFixtures;

namespace ProductHub.UnitTests.Services
{
    public class ProductsServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductValidator _productValidator;
        private readonly ProductsService _productsService;
        private readonly Fixture _fixture;

        public ProductsServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _productValidator = new ProductValidator();
            _productsService = new ProductsService(_mockProductRepository.Object, _productValidator);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetProductsAsync_ValidInput_ShouldReturnProducts()
        {
            // Arrange
            var (pageNumber, pageSize, searchTerm, sort) = ProductHubFixtures.GeneratePaginationData(PaginationScenario.AllValid);
            var expectedProducts = _fixture.Build<Product>()
                                          .Without(p => p.Id)
                                          .Do(x => x.Id = ObjectId.GenerateNewId().ToString())
                                          .CreateMany(50)
                                          .ToList();

            _mockProductRepository.Setup(repo => repo.GetProductsAsync(pageNumber,
                                                                       pageSize,
                                                                       searchTerm,
                                                                       sort))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productsService.GetProductsAsync(pageNumber,
                                                                 pageSize,
                                                                 searchTerm,
                                                                 sort);

            // Assert
            result.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async Task GetProductsAsync_DeafaultInputParams_ShouldReturnProducts()
        {
            // Arrange
            var (pageNumber, pageSize, searchTerm, sort) = ProductHubFixtures.GeneratePaginationData(PaginationScenario.DefaultParameters);
            var expectedProducts = _fixture.Build<Product>()
                                          .Without(p => p.Id)
                                          .Do(x => x.Id = ObjectId.GenerateNewId().ToString())
                                          .CreateMany(50)
                                          .ToList();

            _mockProductRepository.Setup(repo => repo.GetProductsAsync(pageNumber,
                                                                       pageSize,
                                                                       searchTerm,
                                                                       sort))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productsService.GetProductsAsync(pageNumber,
                                                                 pageSize,
                                                                 searchTerm,
                                                                 sort);

            // Assert
            result.Should().BeEquivalentTo(expectedProducts);
        }

        [Fact]
        public async Task GetProductsAsync_InvalidPageNumber_ShouldThrowArgumentException()
        {
            // Arrange
            var (pageNumber, pageSize, searchTerm, sort) = ProductHubFixtures.GeneratePaginationData(PaginationScenario.InvalidPageNumber);


            // Act
            Func<Task> act = async () => await _productsService.GetProductsAsync(pageNumber,
                                                                 pageSize,
                                                                 searchTerm,
                                                                 sort);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage(string.Format(ProductHubResponse.InvalidPageNumber, pageNumber));
            _mockProductRepository.Verify(repo => repo.GetProductsAsync(pageNumber, pageSize, searchTerm, sort), Times.Never);
        }

        [Fact]
        public async Task GetProductsAsync_InvalidPageSize_ShouldThrowArgumentException()
        {
            // Arrange
            var (pageNumber, pageSize, searchTerm, sort) = ProductHubFixtures.GeneratePaginationData(PaginationScenario.InvalidPageSize);


            // Act
            Func<Task> act = async () => await _productsService.GetProductsAsync(pageNumber,
                                                                 pageSize,
                                                                 searchTerm,
                                                                 sort);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage(string.Format(ProductHubResponse.InvalidPageSize, pageSize));
            _mockProductRepository.Verify(repo => repo.GetProductsAsync(pageNumber, pageSize, searchTerm, sort), Times.Never);
        }

        [Fact]
        public async Task GetProductsAsync_InvalidSortParam_ShouldThrowArgumentException()
        {
            // Arrange
            var (pageNumber, pageSize, searchTerm, sort) = ProductHubFixtures.GeneratePaginationData(PaginationScenario.InvalidSortOrder);


            // Act
            Func<Task> act = async () => await _productsService.GetProductsAsync(pageNumber,
                                                                 pageSize,
                                                                 searchTerm,
                                                                 sort);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage(string.Format(ProductHubResponse.InvalidSortParameter, sort));
            _mockProductRepository.Verify(repo => repo.GetProductsAsync(pageNumber, pageSize, searchTerm, sort), Times.Never);
        }

        [Fact]
        public async Task GetProductAsync_ValidId_ShouldReturnProduct()
        {
            // Arrange
            var expectedProduct = _fixture.Build<Product>()
                                  .Without(p => p.Id)
                                  .Do(x => x.Id = ObjectId.GenerateNewId().ToString()).Create();

            _mockProductRepository.Setup(repo => repo.GetProductAsync(expectedProduct.Id))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productsService.GetProductAsync(expectedProduct.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedProduct);
            _mockProductRepository.Verify(repo => repo.GetProductAsync(expectedProduct.Id), Times.Once);
        }

        [Fact]
        public async Task GetProductAsync_InvalidProductId_ShouldThrowArgumentException()
        {
            // Arrange
            var productId = _fixture.Create<string>();

            // Act
            Func<Task> act = async () => await _productsService.GetProductAsync(productId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage(ProductHubResponse.InvalidProductId);
            _mockProductRepository.Verify(repo => repo.GetProductAsync(productId), Times.Never);
        }

        [Fact]
        public async Task AddProductAsync_ValidProduct_ShouldAddProduct()
        {
            // Arrange
            var product = _fixture.Build<Product>()
                                  .Without(p => p.Id)
                                  .Do(x => x.Id = ObjectId.GenerateNewId().ToString()).Create();
            _mockProductRepository.Setup(repo => repo.AddProductAsync(product));

            // Act
            var result = await _productsService.AddProductAsync(product);

            // Assert
            result.Should().BeEquivalentTo(product);
            _mockProductRepository.Verify(repo => repo.AddProductAsync(product), Times.Once);
        }

        [Fact]
        public async Task AddProductAsync_ProductAlreadyExists_ShouldThrowArgumentException()
        {
            // Arrange
            var product = _fixture.Build<Product>()
                                  .Without(p => p.Id)
                                  .Do(x => x.Id = ObjectId.GenerateNewId().ToString()).Create();
            _mockProductRepository.Setup(repo => repo.GetProductByNameAsync(product.Name))
                .ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _productsService.AddProductAsync(product);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage(string.Format(ProductHubResponse.ProductAlreadyExists, product.Name));
            _mockProductRepository.Verify(repo => repo.AddProductAsync(product), Times.Never);
        }

        [Fact]
        public async Task AddProductAsync_Failure_ShouldThrowValidationException()
        {
            // Arrange
            var product = new Product
            {

            };

            // Act
            Func<Task> act = async () => await _productsService.AddProductAsync(product);

            // Assert
            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*" + DomainConstants.ProductNameValidation + "*")
                .WithMessage("*" + DomainConstants.ProductDescriptionValidation + "*")
                .WithMessage("*" + DomainConstants.PriceValidation + "*")
                .WithMessage("*" + DomainConstants.UnitsValidation + "*");
            _mockProductRepository.Verify(repo => repo.AddProductAsync(product), Times.Never);
        }

        [Fact]
        public async Task UpdateProductAsync_ValidIdAndProduct_ShouldUpdateProduct()
        {
            // Arrange
            var product = _fixture.Build<Product>()
                                  .Without(p => p.Id)
                                  .Do(x => x.Id = ObjectId.GenerateNewId().ToString()).Create();
            var productId = product.Id;

            _mockProductRepository.Setup(repo => repo.GetProductAsync(productId))
                .ReturnsAsync(product);

            _mockProductRepository.Setup(repo => repo.UpdateProductAsync(productId, product))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productsService.UpdateProductAsync(productId, product);

            // Assert
            result.Should().BeEquivalentTo(product);
            _mockProductRepository.Verify(repo => repo.UpdateProductAsync(productId, product), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_InValidId_ShouldThrowArgumentException()
        {
            // Arrange
            var product = _fixture.Build<Product>()
                                  .Without(p => p.Id)
                                  .Do(x => x.Id = ObjectId.GenerateNewId().ToString()).Create();
            var productId = _fixture.Create<string>();

            // Act
            Func<Task> act = async () => await _productsService.UpdateProductAsync(productId, product);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage(ProductHubResponse.InvalidProductId);
            _mockProductRepository.Verify(repo => repo.UpdateProductAsync(productId, product), Times.Never);
        }

        [Fact]
        public async Task UpdateProductAsync_Failure_ShouldThrowProductNotFoundException()
        {
            // Arrange
            var product = _fixture.Build<Product>()
                                  .Without(p => p.Id)
                                  .Do(x => x.Id = ObjectId.GenerateNewId().ToString()).Create();
            var productId = ObjectId.GenerateNewId().ToString();

            // Act
            Func<Task> act = async () => await _productsService.UpdateProductAsync(productId, product);

            // Assert
            await act.Should().ThrowAsync<ProductNotFoundException>()
                .WithMessage(ProductHubResponse.ProductNotFound);
            _mockProductRepository.Verify(repo => repo.UpdateProductAsync(productId, product), Times.Never);
        }

        [Fact]
        public async Task UpdateProductAsync_Failure_ShouldThrowValidationException()
        {
            // Arrange
            var product = new Product
            {
                Id = ObjectId.GenerateNewId().ToString(),
            };
            var productId = product.Id;

            // Act
            Func<Task> act = async () => await _productsService.UpdateProductAsync(productId, product);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*" + DomainConstants.ProductNameValidation + "*")
                .WithMessage("*" + DomainConstants.ProductDescriptionValidation + "*")
                .WithMessage("*" + DomainConstants.PriceValidation + "*")
                .WithMessage("*" + DomainConstants.UnitsValidation + "*");
            _mockProductRepository.Verify(repo => repo.UpdateProductAsync(productId, product), Times.Never);
        }

        [Fact]
        public async Task DeleteProductAsync_Success_ShouldDeleteProduct()
        {
            // Arrange
            var productId = ObjectId.GenerateNewId().ToString();
            var existingProduct = _fixture.Create<Product>();

            _mockProductRepository.Setup(repo => repo.GetProductAsync(productId))
                .ReturnsAsync(existingProduct);

            // Act
            await _productsService.DeleteProductAsync(productId);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_Failure_ShouldThrowProductNotFoundException()
        {
            // Arrange
            var productId = ObjectId.GenerateNewId().ToString();

            // Act
            Func<Task> act = async () => await _productsService.DeleteProductAsync(productId);

            // Assert
            await act.Should().ThrowAsync<ProductNotFoundException>()
                .WithMessage(ProductHubResponse.ProductNotFound);
            _mockProductRepository.Verify(repo => repo.DeleteProductAsync(productId), Times.Never);
        }

        [Fact]
        public async Task DeleteProductAsync_Failure_ShouldThrowArgumentNullException()
        {
            // Arrange
            var productId = _fixture.Create<string>();

            // Act
            Func<Task> act = async () => await _productsService.DeleteProductAsync(productId);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            _mockProductRepository.Verify(repo => repo.DeleteProductAsync(productId), Times.Never);
        }
    }
}


using FluentValidation;
using MongoDB.Bson;
using ProductHub.Application.Common;
using ProductHub.Application.Contracts.Products;
using ProductHub.Domain.Exceptions;
using ProductHub.Domain.Products;
using ProductHub.Domain.Validation;

namespace ProductHub.Api.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductRepository _productRepository;
        private readonly ProductValidator _productValidator;

        public ProductsService(
            IProductRepository productRepository,
            ProductValidator productValidator
            )
        {
            _productRepository = productRepository;
            _productValidator = productValidator;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            var validationResult = await _productValidator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var existingProduct = await _productRepository.GetProductByNameAsync(product.Name);
            if (existingProduct != null)
            {
                throw new ArgumentException(string.Format(ProductHubResponse.ProductAlreadyExists, product.Name));
            }

            await _productRepository.AddProductAsync(product);
            return product;
        }

        public async Task<Product> GetProductAsync(string Id)
        {
            if (string.IsNullOrEmpty(Id) || !ObjectId.TryParse(Id, out _))
            {
                throw new ArgumentException(ProductHubResponse.InvalidProductId);
            }

            return await _productRepository.GetProductAsync(Id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, string? searchTerm, string? sort)
        {
            if (pageNumber < 1)
            {
                throw new ArgumentException(string.Format(ProductHubResponse.InvalidPageNumber, pageNumber));
            }

            if (pageSize < 1)
            {
                throw new ArgumentException(string.Format(ProductHubResponse.InvalidPageSize, pageSize));
            }

            if (!string.IsNullOrEmpty(sort) && sort != "asc" && sort != "desc")
            {
                throw new ArgumentException(string.Format(ProductHubResponse.InvalidSortParameter, sort));
            }

            return await _productRepository.GetProductsAsync(pageNumber, pageSize, searchTerm, sort);
        }

        public async Task<Product> UpdateProductAsync(string Id, Product product)
        {
            var validationResult = await _productValidator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (!ObjectId.TryParse(Id, out _))
            {
                throw new ArgumentException(ProductHubResponse.InvalidProductId);
            }

            var existingProduct = await _productRepository.GetProductAsync(Id);

            if (existingProduct == null)
            {
                throw new ProductNotFoundException(ProductHubResponse.ProductNotFound);
            }

            await _productRepository.UpdateProductAsync(Id, product);
            return product;
        }

        public async Task DeleteProductAsync(string Id)
        {
            if (!ObjectId.TryParse(Id, out _))
            {
                throw new ArgumentNullException(nameof(Id), ProductHubResponse.InvalidProductId);
            }

            var product = await _productRepository.GetProductAsync(Id);

            if (product == null)
            {
                throw new ProductNotFoundException(ProductHubResponse.ProductNotFound);
            }

            await _productRepository.DeleteProductAsync(Id);
        }
    }
}

using ProductHub.Domain.Products;

namespace ProductHub.Application.Contracts.Products
{
    public interface IProductsService
    {
        public Task<Product> AddProductAsync(Product product);
        public Task<Product> GetProductAsync(string Id);
        public Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, string? searchTerm, string? sort);
        public Task<Product> UpdateProductAsync(string Id, Product product);
        public Task DeleteProductAsync(string Id);
    }
}

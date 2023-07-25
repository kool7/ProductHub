using ProductHub.Api.Models;

namespace ProductHub.Api.Contracts
{
    public interface IProductsService
    {
        public Task<Product> AddProductAsync(Product product);
        public Task<Product> GetProductAsync(string Id);
        public Task<IEnumerable<Product>> GetProductsAsync();
        public Task DeleteProductAsync(string Id);
    }
}

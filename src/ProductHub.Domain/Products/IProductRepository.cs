namespace ProductHub.Domain.Products
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, string? searchTerm, string? sort);
        public Task AddProductAsync(Product product);
        public Task<Product> GetProductAsync(string Id);
        public Task<Product> GetProductByNameAsync(string Name);
        public Task UpdateProductAsync(string Id, Product product);
        public Task DeleteProductAsync(string Id);
    }
}

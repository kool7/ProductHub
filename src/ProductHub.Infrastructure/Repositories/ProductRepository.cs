using MongoDB.Bson;
using MongoDB.Driver;
using ProductHub.Domain.Products;
using ProductHub.Infrastructure.Utility;

namespace ProductHub.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _productDbContext;

        public ProductRepository(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        public async Task AddProductAsync(Product product)
        {
            await _productDbContext.Products.InsertOneAsync(product);
        }

        public async Task<Product> GetProductAsync(string Id)
        {
            var product = await _productDbContext.Products.Find(p => p.Id == Id).FirstOrDefaultAsync();
            return product;
        }

        public async Task<Product> GetProductByNameAsync(string Name)
        {
            var filter = Builders<Product>.Filter.Eq(product => product.Name, Name);
            return await _productDbContext.Products.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, string? searchTerm, string? sort)
        {
            var filter = Builders<Product>.Filter.Empty;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                filter = Builders<Product>.Filter.Regex(nameof(Product.Name), new BsonRegularExpression(searchTerm, "i")) |
                    Builders<Product>.Filter.Regex(nameof(Product.Description), new BsonRegularExpression(searchTerm, "i"));
            }

            var filteredResults = _productDbContext.Products
                .Find(filter);

            var results = filteredResults.Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize);

            if (sort == "asc")
            {
                results = results.SortBy(product => product.Units);
            }
            else if (sort == "desc")
            {
                results = results.SortByDescending(product => product.Units);
            }

            return await results.ToListAsync();
        }

        public async Task UpdateProductAsync(string Id, Product product)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, Id);
            await _productDbContext.Products.ReplaceOneAsync(filter, product);
        }

        public async Task DeleteProductAsync(string Id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, Id);
            await _productDbContext.Products.DeleteOneAsync(filter);
        }
    }
}

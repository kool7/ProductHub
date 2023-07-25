using MongoDB.Bson;
using MongoDB.Driver;
using ProductHub.Api.Contracts;
using ProductHub.Api.Data;
using ProductHub.Api.Models;

namespace ProductHub.Api.Services
{
    public class ProductsService : IProductsService
    {
        private readonly ProductDbContext _productDbContext;

        public ProductsService(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            await _productDbContext.Products.InsertOneAsync(product);
            return product;
        }

        public async Task DeleteProductAsync(string Id)
        {
            await _productDbContext.Products.DeleteOneAsync(product => product.Id == Id);
        }

        public async Task<Product> GetProductAsync(string Id)
        {
            var product = await _productDbContext.Products.Find(p => p.Id == Id).FirstOrDefaultAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _productDbContext.Products.Find(new BsonDocument()).ToListAsync();
        }
    }
}

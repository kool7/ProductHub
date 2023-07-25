using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductHub.Api.Models;
using ProductHub.Api.Utility;

namespace ProductHub.Api.Data
{
    public class ProductDbContext
    {
        private readonly IMongoDatabase _database;

        public ProductDbContext(IOptions<MongoDBSettings> options)
        {
            var connectionString = options.Value.ConnectionURI;
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(options.Value.DatabaseName);
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
    }
}

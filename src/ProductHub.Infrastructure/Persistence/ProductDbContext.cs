using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductHub.Domain.Products;

namespace ProductHub.Infrastructure.Utility;

public class ProductDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IOptions<MongoDBSettings> _options;

    public ProductDbContext(IOptions<MongoDBSettings> options)
    {
        var connectionString = options.Value.ConnectionURI;
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(options.Value.DatabaseName);
        _options = options;
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>(_options.Value.CollectionName);
}

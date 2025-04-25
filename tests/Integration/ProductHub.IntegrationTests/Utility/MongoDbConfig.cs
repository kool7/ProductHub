using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductHub.Domain.Products;
using ProductHub.Infrastructure.Utility;
using System.Text;

namespace ProductHub.IntegrationTests.Utility
{
    internal static class MongoDbConfig
    {
        private static Fixture _fixture = new Fixture();
        public static List<Product> seedProducts;

        public static void InitializeDbForTests(WebApplicationFactory<Program> appFactory)
        {
            using var scope = appFactory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            db.Products.DeleteMany(Builders<Product>.Filter.Empty);
            db.Products.InsertMany(GenerateSeedProducts());
        }

        public static List<Product> GenerateSeedProducts()
        {
            var productFixtures = _fixture.Build<Product>()
                                          .Without(p => p.Id)
                                          .Do(x => x.Id = ObjectId.GenerateNewId().ToString())
                                          .CreateMany(50)
                                          .ToList();

            var products = new List<Product>();
            products.AddRange(productFixtures);
            seedProducts = products;
            return products;
        }

        public static string CreateQueryString(object queryParameters)
        {
            var queryString = new StringBuilder();

            var properties = queryParameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(queryParameters);
                if (value != null)
                {
                    queryString.Append(queryString.Length > 0 ? "&" : "?");
                    queryString.Append($"{property.Name}={value}");
                }
            }

            return queryString.ToString();
        }
    }
}

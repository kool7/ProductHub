using Microsoft.Extensions.DependencyInjection;
using ProductHub.Domain.Products;
using ProductHub.Infrastructure.Repositories;
using ProductHub.Infrastructure.Utility;
using Microsoft.Extensions.Configuration;

namespace ProductHub.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        services.Configure<MongoDBSettings>(configurationManager.GetSection("MongoDB"));
        services.AddSingleton<ProductDbContext>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}

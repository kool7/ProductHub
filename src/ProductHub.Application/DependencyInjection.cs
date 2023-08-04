using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductHub.Api.Services;
using ProductHub.Application.Contracts.Products;
using ProductHub.Domain.Validation;

namespace ProductHub.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddValidatorsFromAssemblyContaining<ProductValidator>();
        return services;
    }
}

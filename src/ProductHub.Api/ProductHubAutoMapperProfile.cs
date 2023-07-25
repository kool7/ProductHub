using AutoMapper;
using ProductHub.Api.Contracts;
using ProductHub.Api.Models;

namespace ProductHub.Api
{
    public class ProductHubAutoMapperProfile: Profile
    {
        public ProductHubAutoMapperProfile()
        {
            CreateMap<Product, ReadProductDto>();
            CreateMap<CreateProductDto, Product>();
        }
    }
}

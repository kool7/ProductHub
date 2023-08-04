using AutoMapper;
using ProductHub.Application.Contracts.Products;
using ProductHub.Domain.Products;

namespace ProductHub.Api
{
    public class ProductHubAutoMapperProfile : Profile
    {
        public ProductHubAutoMapperProfile()
        {
            CreateMap<Product, ReadProductDto>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.Id,
                                   opt => opt.MapFrom((src, dest, destMember, context) => context.Items["Id"]));
            CreateMap<Product, UpdateProductDto>();
        }
    }
}

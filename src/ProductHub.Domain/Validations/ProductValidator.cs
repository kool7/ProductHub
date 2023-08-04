using FluentValidation;
using ProductHub.Domain.Common;
using ProductHub.Domain.Products;

namespace ProductHub.Domain.Validation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(product => product.Name)
                .NotNull()
                .NotEmpty()
                .WithMessage(DomainConstants.ProductNameValidation);
            RuleFor(product => product.Description)
                .NotNull()
                .NotEmpty()
                .WithMessage(DomainConstants.ProductDescriptionValidation);
            RuleFor(product => product.Price)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(DomainConstants.PriceValidation);
            RuleFor(product => product.Units)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(DomainConstants.UnitsValidation);
        }
    }
}

using FluentValidation;

namespace Ecommerce.Catalog.Application.Products.UpdateProductPrice;

public sealed class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEmpty();

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0);
    }
}

using Ecommerce.Catalog.Domain.Products;
using FluentValidation;

namespace Ecommerce.Catalog.Application.Products.UpdateProductDetails;

public sealed class UpdateProductDetailsCommandValidator : AbstractValidator<UpdateProductDetailsCommand>
{
    public UpdateProductDetailsCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(ProductName.MaxLength);

        RuleFor(command => command.Description)
            .MaximumLength(Product.DescriptionMaxLength);
    }
}

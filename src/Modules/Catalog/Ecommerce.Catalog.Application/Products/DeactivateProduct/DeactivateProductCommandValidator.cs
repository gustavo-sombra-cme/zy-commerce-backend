using FluentValidation;

namespace Ecommerce.Catalog.Application.Products.DeactivateProduct;

public sealed class DeactivateProductCommandValidator : AbstractValidator<DeactivateProductCommand>
{
    public DeactivateProductCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEmpty();
    }
}

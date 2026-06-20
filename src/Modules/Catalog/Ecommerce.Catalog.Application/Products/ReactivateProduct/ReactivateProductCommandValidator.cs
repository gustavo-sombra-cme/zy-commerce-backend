using FluentValidation;

namespace Ecommerce.Catalog.Application.Products.ReactivateProduct;

public sealed class ReactivateProductCommandValidator : AbstractValidator<ReactivateProductCommand>
{
    public ReactivateProductCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEmpty();
    }
}

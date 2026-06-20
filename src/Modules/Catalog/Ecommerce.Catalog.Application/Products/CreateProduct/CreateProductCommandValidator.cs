using Ecommerce.Catalog.Domain.Products;
using FluentValidation;

namespace Ecommerce.Catalog.Application.Products.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Sku)
            .NotEmpty()
            .MaximumLength(Sku.MaxLength)
            .Must(BeValidSku)
            .WithMessage("SKU can contain only uppercase letters, numbers, hyphen, or underscore.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(ProductName.MaxLength);

        RuleFor(command => command.Description)
            .MaximumLength(Product.DescriptionMaxLength);

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0);
    }

    private static bool BeValidSku(string sku)
    {
        try
        {
            return !string.IsNullOrWhiteSpace(sku)
                && Sku.Create(sku).Value == sku.Trim();
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}

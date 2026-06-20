using Ecommerce.Orders.Domain.Orders;
using FluentValidation;

namespace Ecommerce.Orders.Application.Orders.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.BuyerId)
            .NotEmpty();

        RuleFor(command => command.Lines)
            .NotEmpty();

        RuleForEach(command => command.Lines)
            .SetValidator(new CreateOrderLineCommandValidator());
    }
}

public sealed class CreateOrderLineCommandValidator : AbstractValidator<CreateOrderLineCommand>
{
    public CreateOrderLineCommandValidator()
    {
        RuleFor(line => line.ProductId)
            .NotEmpty();

        RuleFor(line => line.ProductSku)
            .NotEmpty()
            .MaximumLength(OrderLine.ProductSkuMaxLength);

        RuleFor(line => line.ProductName)
            .NotEmpty()
            .MaximumLength(OrderLine.ProductNameMaxLength);

        RuleFor(line => line.UnitPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(line => line.Quantity)
            .GreaterThan(0);
    }
}

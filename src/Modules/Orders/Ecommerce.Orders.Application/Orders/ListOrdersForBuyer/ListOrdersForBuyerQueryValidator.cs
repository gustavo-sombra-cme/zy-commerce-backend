using FluentValidation;

namespace Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

public sealed class ListOrdersForBuyerQueryValidator : AbstractValidator<ListOrdersForBuyerQuery>
{
    public const int MaxPageSize = 100;

    public ListOrdersForBuyerQueryValidator()
    {
        RuleFor(query => query.BuyerId)
            .NotEmpty();

        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1)
            .When(query => query.PageNumber.HasValue);

        RuleFor(query => query.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(MaxPageSize)
            .When(query => query.PageSize.HasValue);
    }
}

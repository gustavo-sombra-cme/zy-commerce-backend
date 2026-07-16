using FluentValidation;

namespace Ecommerce.Catalog.Application.Products.SearchProducts;

public sealed class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public const int MaxPageSize = 100;

    public SearchProductsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1)
            .When(query => query.PageNumber.HasValue);

        RuleFor(query => query.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(MaxPageSize)
            .When(query => query.PageSize.HasValue);

        RuleFor(query => query.MaximumPrice)
            .GreaterThanOrEqualTo(0)
            .When(query => query.MaximumPrice.HasValue);
    }
}

using Ecommerce.Catalog.Application.Abstractions;
using MediatR;

namespace Ecommerce.Catalog.Application.Products.SearchProducts;

public sealed class SearchProductsQueryHandler(IProductReadRepository productReadRepository)
    : IRequestHandler<SearchProductsQuery, PagedResult<ProductListItemDto>>
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;

    public Task<PagedResult<ProductListItemDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var normalizedQuery = request with
        {
            SearchTerm = NormalizeSearchTerm(request.SearchTerm),
            PageNumber = request.PageNumber ?? DefaultPageNumber,
            PageSize = request.PageSize ?? DefaultPageSize
        };

        return productReadRepository.SearchAsync(normalizedQuery, cancellationToken);
    }

    private static string? NormalizeSearchTerm(string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return null;
        }

        return searchTerm.Trim();
    }
}

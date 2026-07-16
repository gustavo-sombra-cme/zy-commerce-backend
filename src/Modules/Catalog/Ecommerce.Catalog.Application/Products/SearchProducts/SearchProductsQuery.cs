using Ecommerce.Catalog.Application.Abstractions;
using MediatR;

namespace Ecommerce.Catalog.Application.Products.SearchProducts;

public sealed record SearchProductsQuery(
    string? SearchTerm,
    bool? IsActive,
    int? PageNumber,
    int? PageSize,
    decimal? MaximumPrice = null) : IRequest<PagedResult<ProductListItemDto>>;

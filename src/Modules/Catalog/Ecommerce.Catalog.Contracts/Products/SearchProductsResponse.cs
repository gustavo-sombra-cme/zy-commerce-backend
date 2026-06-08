namespace Ecommerce.Catalog.Contracts.Products;

public sealed record SearchProductsResponse(
    IReadOnlyCollection<ProductListItemResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

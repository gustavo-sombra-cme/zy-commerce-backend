namespace Ecommerce.Catalog.Application.Products.SearchProducts;

public sealed record ProductListItemDto(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt);

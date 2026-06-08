namespace Ecommerce.Catalog.Contracts.Products;

public sealed record ProductListItemResponse(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt);

namespace Ecommerce.Catalog.Contracts.Products;

public sealed record GetProductByIdResponse(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

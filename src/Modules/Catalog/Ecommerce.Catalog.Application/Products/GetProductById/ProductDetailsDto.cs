namespace Ecommerce.Catalog.Application.Products.GetProductById;

public sealed record ProductDetailsDto(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

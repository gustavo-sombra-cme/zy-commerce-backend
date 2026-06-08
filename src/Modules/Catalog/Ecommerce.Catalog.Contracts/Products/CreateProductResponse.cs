namespace Ecommerce.Catalog.Contracts.Products;

public sealed record CreateProductResponse(
    Guid ProductId,
    string Sku,
    string Name);

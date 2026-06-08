namespace Ecommerce.Catalog.Contracts.Products;

public sealed record CreateProductRequest(
    string Sku,
    string Name,
    string? Description);

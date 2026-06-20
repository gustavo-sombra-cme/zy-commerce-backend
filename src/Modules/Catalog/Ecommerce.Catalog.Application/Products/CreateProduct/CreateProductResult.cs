namespace Ecommerce.Catalog.Application.Products.CreateProduct;

public sealed record CreateProductResult(
    Guid ProductId,
    string Sku,
    string Name);

namespace Ecommerce.Catalog.Contracts.Products;

public sealed record UpdateProductDetailsRequest(
    string Name,
    string? Description);

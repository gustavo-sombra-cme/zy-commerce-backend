using MediatR;

namespace Ecommerce.Catalog.Application.Products.CreateProduct;

public sealed record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    decimal Price) : IRequest<CreateProductResult>;

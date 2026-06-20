using MediatR;

namespace Ecommerce.Catalog.Application.Products.UpdateProductDetails;

public sealed record UpdateProductDetailsCommand(
    Guid ProductId,
    string Name,
    string? Description) : IRequest;

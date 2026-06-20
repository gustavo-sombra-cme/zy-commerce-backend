using MediatR;

namespace Ecommerce.Catalog.Application.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDetailsDto?>;

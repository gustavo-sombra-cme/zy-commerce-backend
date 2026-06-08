using MediatR;

namespace Ecommerce.Catalog.Application.Products.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid ProductId) : IRequest;

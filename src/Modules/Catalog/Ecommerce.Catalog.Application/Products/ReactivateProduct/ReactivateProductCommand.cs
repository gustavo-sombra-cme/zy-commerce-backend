using MediatR;

namespace Ecommerce.Catalog.Application.Products.ReactivateProduct;

public sealed record ReactivateProductCommand(Guid ProductId) : IRequest;

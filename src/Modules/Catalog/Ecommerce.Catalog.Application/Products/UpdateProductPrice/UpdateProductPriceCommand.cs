using MediatR;

namespace Ecommerce.Catalog.Application.Products.UpdateProductPrice;

public sealed record UpdateProductPriceCommand(Guid ProductId, decimal Price) : IRequest;

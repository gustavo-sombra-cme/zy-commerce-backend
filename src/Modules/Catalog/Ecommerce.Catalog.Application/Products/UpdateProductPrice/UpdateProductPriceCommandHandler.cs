using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Domain.Products;
using MediatR;

namespace Ecommerce.Catalog.Application.Products.UpdateProductPrice;

public sealed class UpdateProductPriceCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductPriceCommand>
{
    public async Task Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var productId = ProductId.From(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{request.ProductId}' was not found.");

        product.UpdatePrice(request.Price, DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

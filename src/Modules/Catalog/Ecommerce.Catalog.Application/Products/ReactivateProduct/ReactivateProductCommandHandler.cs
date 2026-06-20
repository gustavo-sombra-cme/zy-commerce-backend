using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Domain.Products;
using MediatR;

namespace Ecommerce.Catalog.Application.Products.ReactivateProduct;

public sealed class ReactivateProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<ReactivateProductCommand>
{
    public async Task Handle(ReactivateProductCommand request, CancellationToken cancellationToken)
    {
        var productId = ProductId.From(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{request.ProductId}' was not found.");

        product.Reactivate(DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

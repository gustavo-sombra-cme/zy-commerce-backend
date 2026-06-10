using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Domain.Products;
using MediatR;

namespace Ecommerce.Catalog.Application.Products.UpdateProductDetails;

public sealed class UpdateProductDetailsCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductDetailsCommand>
{
    public async Task Handle(UpdateProductDetailsCommand request, CancellationToken cancellationToken)
    {
        var productId = ProductId.From(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product '{request.ProductId}' was not found.");

        var name = ProductName.Create(request.Name);

        product.UpdateDetails(name, request.Description, DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

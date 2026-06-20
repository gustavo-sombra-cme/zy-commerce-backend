using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Domain.Products;
using MediatR;

namespace Ecommerce.Catalog.Application.Products.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var sku = Sku.Create(request.Sku);

        if (await productRepository.ExistsBySkuAsync(sku, cancellationToken))
        {
            throw new DuplicateSkuException(sku.Value);
        }

        var name = ProductName.Create(request.Name);
        var product = Product.Create(sku, name, request.Description, request.Price, DateTimeOffset.UtcNow);

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id.Value, product.Sku.Value, product.Name.Value);
    }
}

using MediatR;

namespace Ecommerce.Catalog.Application.Products.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductReadRepository productReadRepository)
    : IRequestHandler<GetProductByIdQuery, ProductDetailsDto?>
{
    public Task<ProductDetailsDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.ProductId == Guid.Empty)
        {
            return Task.FromResult<ProductDetailsDto?>(null);
        }

        return productReadRepository.GetByIdAsync(request.ProductId, cancellationToken);
    }
}

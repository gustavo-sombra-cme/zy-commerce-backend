using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products.SearchProducts;

namespace Ecommerce.Catalog.Application.Products;

public interface IProductReadRepository
{
    Task<ProductDetailsDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken);

    Task<PagedResult<ProductListItemDto>> SearchAsync(SearchProductsQuery query, CancellationToken cancellationToken);
}

using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.Application.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken);

    Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken);

    Task AddAsync(Product product, CancellationToken cancellationToken);
}

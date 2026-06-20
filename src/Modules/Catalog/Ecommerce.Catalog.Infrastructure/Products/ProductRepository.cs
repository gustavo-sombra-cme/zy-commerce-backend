using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Domain.Products;
using Ecommerce.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Catalog.Infrastructure.Products;

public sealed class ProductRepository(CatalogDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken) =>
        dbContext.Products.SingleOrDefaultAsync(product => product.Id == productId, cancellationToken);

    public Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken) =>
        dbContext.Products.AnyAsync(product => product.Sku == sku, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken) =>
        await dbContext.Products.AddAsync(product, cancellationToken);
}

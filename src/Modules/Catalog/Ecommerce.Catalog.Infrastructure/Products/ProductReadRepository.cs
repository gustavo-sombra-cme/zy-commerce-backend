using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Catalog.Domain.Products;
using Ecommerce.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Catalog.Infrastructure.Products;

public sealed class ProductReadRepository(
    CatalogDbContext dbContext,
    CatalogReadDbContext readDbContext) : IProductReadRepository
{
    public Task<ProductDetailsDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var id = ProductId.From(productId);

        return dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new ProductDetailsDto(
                product.Id.Value,
                product.Sku.Value,
                product.Name.Value,
                product.Description,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<ProductListItemDto>> SearchAsync(
        SearchProductsQuery query,
        CancellationToken cancellationToken)
    {
        var pageNumber = query.PageNumber ?? SearchProductsQueryHandler.DefaultPageNumber;
        var pageSize = query.PageSize ?? SearchProductsQueryHandler.DefaultPageSize;

        var products = readDbContext.Set<ProductSearchReadModel>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchPattern = $"%{query.SearchTerm.Trim()}%";
            products = products.Where(product =>
                EF.Functions.Like(EF.Property<string>(product, nameof(ProductSearchReadModel.Sku)), searchPattern)
                || EF.Functions.Like(EF.Property<string>(product, nameof(ProductSearchReadModel.Name)), searchPattern));
        }

        if (query.IsActive.HasValue)
        {
            products = products.Where(product => product.IsActive == query.IsActive.Value);
        }

        var totalCount = await products.CountAsync(cancellationToken);

        var items = await products
            .OrderByDescending(product => product.CreatedAt)
            .ThenBy(product => EF.Property<string>(product, nameof(ProductSearchReadModel.Name)))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(product => new ProductListItemDto(
                product.Id,
                EF.Property<string>(product, nameof(ProductSearchReadModel.Sku)),
                EF.Property<string>(product, nameof(ProductSearchReadModel.Name)),
                product.Description,
                product.IsActive,
                product.CreatedAt))
            .ToArrayAsync(cancellationToken);

        return new PagedResult<ProductListItemDto>(items, pageNumber, pageSize, totalCount);
    }
}

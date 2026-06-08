using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products.SearchProducts;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_ReturnsProductDetails()
    {
        var productId = Guid.NewGuid();
        var expected = new ProductDetailsDto(
            productId,
            "SKU-001",
            "Test Product",
            "Description",
            true,
            DateTimeOffset.UtcNow,
            null);
        var repository = new FakeProductReadRepository(expected);
        var handler = new GetProductByIdQueryHandler(repository);

        var result = await handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        Assert.Equal(expected, result);
        Assert.Equal(productId, repository.RequestedProductId);
    }

    [Fact]
    public async Task Handle_WithMissingProduct_ReturnsNull()
    {
        var repository = new FakeProductReadRepository(product: null);
        var handler = new GetProductByIdQueryHandler(repository);

        var result = await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyProductId_ReturnsNullWithoutCallingRepository()
    {
        var repository = new FakeProductReadRepository(product: null);
        var handler = new GetProductByIdQueryHandler(repository);

        var result = await handler.Handle(new GetProductByIdQuery(Guid.Empty), CancellationToken.None);

        Assert.Null(result);
        Assert.False(repository.WasCalled);
    }

    private sealed class FakeProductReadRepository(ProductDetailsDto? product) : IProductReadRepository
    {
        public Guid? RequestedProductId { get; private set; }

        public bool WasCalled { get; private set; }

        public Task<ProductDetailsDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            WasCalled = true;
            RequestedProductId = productId;
            return Task.FromResult(product);
        }

        public Task<PagedResult<ProductListItemDto>> SearchAsync(
            SearchProductsQuery query,
            CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<ProductListItemDto>(
                Array.Empty<ProductListItemDto>(),
                query.PageNumber ?? SearchProductsQueryHandler.DefaultPageNumber,
                query.PageSize ?? SearchProductsQueryHandler.DefaultPageSize,
                0));
    }
}

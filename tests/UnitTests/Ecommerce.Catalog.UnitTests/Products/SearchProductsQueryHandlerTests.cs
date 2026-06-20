using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class SearchProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNullPagination_AppliesDefaults()
    {
        var repository = new FakeProductReadRepository();
        var handler = new SearchProductsQueryHandler(repository);

        await handler.Handle(new SearchProductsQuery(null, null, null, null), CancellationToken.None);

        Assert.NotNull(repository.LastQuery);
        Assert.Equal(SearchProductsQueryHandler.DefaultPageNumber, repository.LastQuery.PageNumber);
        Assert.Equal(SearchProductsQueryHandler.DefaultPageSize, repository.LastQuery.PageSize);
    }

    [Fact]
    public async Task Handle_WithWhitespaceSearchTerm_NormalizesToNull()
    {
        var repository = new FakeProductReadRepository();
        var handler = new SearchProductsQueryHandler(repository);

        await handler.Handle(new SearchProductsQuery("   ", null, 1, 20), CancellationToken.None);

        Assert.Null(repository.LastQuery?.SearchTerm);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_TrimsSearchTerm()
    {
        var repository = new FakeProductReadRepository();
        var handler = new SearchProductsQueryHandler(repository);

        await handler.Handle(new SearchProductsQuery(" SKU ", true, 2, 10), CancellationToken.None);

        Assert.Equal("SKU", repository.LastQuery?.SearchTerm);
        Assert.True(repository.LastQuery?.IsActive);
        Assert.Equal(2, repository.LastQuery?.PageNumber);
        Assert.Equal(10, repository.LastQuery?.PageSize);
    }

    [Fact]
    public async Task Handle_ReturnsRepositoryResult()
    {
        var expected = new PagedResult<ProductListItemDto>(
            new[]
            {
                new ProductListItemDto(
                    Guid.NewGuid(),
                    "SKU-001",
                    "Test Product",
                    "Description",
                    true,
                    DateTimeOffset.UtcNow)
                {
                    Price = 12.34m
                }
            },
            1,
            20,
            1);
        var repository = new FakeProductReadRepository(expected);
        var handler = new SearchProductsQueryHandler(repository);

        var result = await handler.Handle(new SearchProductsQuery(null, null, 1, 20), CancellationToken.None);

        Assert.Equal(expected, result);
        Assert.Equal(12.34m, Assert.Single(result.Items).Price);
    }

    private sealed class FakeProductReadRepository(PagedResult<ProductListItemDto>? result = null)
        : IProductReadRepository
    {
        public SearchProductsQuery? LastQuery { get; private set; }

        public Task<ProductDetailsDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken) =>
            Task.FromResult<ProductDetailsDto?>(null);

        public Task<PagedResult<ProductListItemDto>> SearchAsync(
            SearchProductsQuery query,
            CancellationToken cancellationToken)
        {
            LastQuery = query;
            return Task.FromResult(result ?? new PagedResult<ProductListItemDto>(
                Array.Empty<ProductListItemDto>(),
                query.PageNumber ?? SearchProductsQueryHandler.DefaultPageNumber,
                query.PageSize ?? SearchProductsQueryHandler.DefaultPageSize,
                0));
        }
    }
}

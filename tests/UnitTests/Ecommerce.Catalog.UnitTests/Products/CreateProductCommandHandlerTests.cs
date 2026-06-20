using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.CreateProduct;
using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesProduct()
    {
        var repository = new FakeProductRepository();
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new CreateProductCommandHandler(repository, unitOfWork);

        var result = await handler.Handle(
            new CreateProductCommand("SKU-001", "Test Product", "Description", 12.34m),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.ProductId);
        Assert.Equal("SKU-001", result.Sku);
        Assert.Equal("Test Product", result.Name);
        var product = Assert.Single(repository.Products);
        Assert.Equal(12.34m, product.Price);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithDuplicateSku_ThrowsAndDoesNotSave()
    {
        var repository = new FakeProductRepository(existingSku: "SKU-001");
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new CreateProductCommandHandler(repository, unitOfWork);

        await Assert.ThrowsAsync<DuplicateSkuException>(() => handler.Handle(
            new CreateProductCommand("SKU-001", "Test Product", null, 12.34m),
            CancellationToken.None));

        Assert.Empty(repository.Products);
        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private sealed class FakeProductRepository(string? existingSku = null) : IProductRepository
    {
        private readonly string? _existingSku = existingSku;

        public List<Product> Products { get; } = [];

        public Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken) =>
            Task.FromResult(Products.SingleOrDefault(product => product.Id == productId));

        public Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken) =>
            Task.FromResult(_existingSku == sku.Value);

        public Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            Products.Add(product);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeCatalogUnitOfWork : ICatalogUnitOfWork
    {
        public bool SaveChangesCalled { get; private set; }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalled = true;
            return Task.CompletedTask;
        }
    }
}

using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.DeactivateProduct;
using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class DeactivateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_DeactivatesProductAndSaves()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new DeactivateProductCommandHandler(repository, unitOfWork);

        await handler.Handle(new DeactivateProductCommand(product.Id.Value), CancellationToken.None);

        Assert.False(product.IsActive);
        Assert.NotNull(product.UpdatedAt);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithInactiveProduct_RemainsInactiveAndSaves()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        product.Deactivate(DateTimeOffset.UtcNow.AddMinutes(1));
        var originalUpdatedAt = product.UpdatedAt;
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new DeactivateProductCommandHandler(repository, unitOfWork);

        await handler.Handle(new DeactivateProductCommand(product.Id.Value), CancellationToken.None);

        Assert.False(product.IsActive);
        Assert.Equal(originalUpdatedAt, product.UpdatedAt);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithMissingProduct_ThrowsAndDoesNotSave()
    {
        var repository = new FakeProductRepository(product: null);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new DeactivateProductCommandHandler(repository, unitOfWork);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new DeactivateProductCommand(Guid.NewGuid()),
            CancellationToken.None));

        Assert.False(unitOfWork.SaveChangesCalled);
    }

    private sealed class FakeProductRepository(Product? product) : IProductRepository
    {
        public Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken) =>
            Task.FromResult(product is not null && product.Id == productId ? product : null);

        public Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken) =>
            Task.FromResult(false);

        public Task AddAsync(Product product, CancellationToken cancellationToken) =>
            Task.CompletedTask;
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

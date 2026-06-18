using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.ReactivateProduct;
using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class ReactivateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithInactiveProduct_ReactivatesProductAndSaves()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var productId = product.Id;
        var sku = product.Sku;
        var deactivatedAt = DateTimeOffset.UtcNow.AddMinutes(-1);
        product.Deactivate(deactivatedAt);
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new ReactivateProductCommandHandler(repository, unitOfWork);

        await handler.Handle(new ReactivateProductCommand(product.Id.Value), CancellationToken.None);

        Assert.True(product.IsActive);
        Assert.NotNull(product.UpdatedAt);
        Assert.True(product.UpdatedAt.Value > deactivatedAt);
        Assert.Equal(productId, product.Id);
        Assert.Equal(sku, product.Sku);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithActiveProduct_RemainsActiveWithoutUpdatingTimestampAndSaves()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            DateTimeOffset.UtcNow);
        var originalUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(-1);
        product.UpdateDetails(ProductName.Create("Updated Product"), "Updated description", originalUpdatedAt);
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new ReactivateProductCommandHandler(repository, unitOfWork);

        await handler.Handle(new ReactivateProductCommand(product.Id.Value), CancellationToken.None);

        Assert.True(product.IsActive);
        Assert.Equal(originalUpdatedAt, product.UpdatedAt);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithMissingProduct_ThrowsAndDoesNotSave()
    {
        var repository = new FakeProductRepository(product: null);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new ReactivateProductCommandHandler(repository, unitOfWork);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new ReactivateProductCommand(Guid.NewGuid()),
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

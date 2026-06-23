using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.UpdateProductPrice;
using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class UpdateProductPriceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_UpdatesPriceAndSaves()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Test Product"),
            null,
            12.34m,
            DateTimeOffset.UtcNow);
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new UpdateProductPriceCommandHandler(repository, unitOfWork);

        await handler.Handle(new UpdateProductPriceCommand(product.Id.Value, 25.559m), CancellationToken.None);

        Assert.Equal(25.56m, product.Price);
        Assert.NotNull(product.UpdatedAt);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithMissingProduct_ThrowsAndDoesNotSave()
    {
        var repository = new FakeProductRepository(product: null);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new UpdateProductPriceCommandHandler(repository, unitOfWork);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateProductPriceCommand(Guid.NewGuid(), 19.99m),
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

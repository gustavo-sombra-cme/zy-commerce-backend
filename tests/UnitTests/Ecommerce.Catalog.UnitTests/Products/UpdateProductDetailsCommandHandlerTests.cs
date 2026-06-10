using Ecommerce.Catalog.Application.Abstractions;
using Ecommerce.Catalog.Application.Products;
using Ecommerce.Catalog.Application.Products.UpdateProductDetails;
using Ecommerce.Catalog.Domain.Products;

namespace Ecommerce.Catalog.UnitTests.Products;

public sealed class UpdateProductDetailsCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_UpdatesProductDetailsAndSaves()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Original Product"),
            "Original description",
            DateTimeOffset.UtcNow);
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new UpdateProductDetailsCommandHandler(repository, unitOfWork);

        await handler.Handle(
            new UpdateProductDetailsCommand(product.Id.Value, "Updated Product", "Updated description"),
            CancellationToken.None);

        Assert.Equal("Updated Product", product.Name.Value);
        Assert.Equal("Updated description", product.Description);
        Assert.Equal("SKU-001", product.Sku.Value);
        Assert.NotNull(product.UpdatedAt);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithWhitespaceDescription_StoresNullDescriptionAndSaves()
    {
        var product = Product.Create(
            Sku.Create("SKU-001"),
            ProductName.Create("Original Product"),
            "Original description",
            DateTimeOffset.UtcNow);
        var repository = new FakeProductRepository(product);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new UpdateProductDetailsCommandHandler(repository, unitOfWork);

        await handler.Handle(
            new UpdateProductDetailsCommand(product.Id.Value, "Updated Product", "   "),
            CancellationToken.None);

        Assert.Null(product.Description);
        Assert.True(unitOfWork.SaveChangesCalled);
    }

    [Fact]
    public async Task Handle_WithMissingProduct_ThrowsAndDoesNotSave()
    {
        var repository = new FakeProductRepository(product: null);
        var unitOfWork = new FakeCatalogUnitOfWork();
        var handler = new UpdateProductDetailsCommandHandler(repository, unitOfWork);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new UpdateProductDetailsCommand(Guid.NewGuid(), "Updated Product", null),
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

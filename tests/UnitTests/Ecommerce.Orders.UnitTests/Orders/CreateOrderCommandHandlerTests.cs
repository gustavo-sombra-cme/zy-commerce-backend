using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Application.Orders;
using Ecommerce.Orders.Application.Orders.CreateOrder;
using Ecommerce.Orders.Domain.Orders;

namespace Ecommerce.Orders.UnitTests.Orders;

public sealed class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistOrder_ForBuyerWithProductSnapshots()
    {
        var repository = new FakeOrderRepository();
        var unitOfWork = new FakeOrdersUnitOfWork();
        var handler = new CreateOrderCommandHandler(repository, unitOfWork);
        var buyerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new CreateOrderCommand(
            buyerId,
            new[]
            {
                new CreateOrderLineCommand(productId, "SKU-1", "Product One", 12.50m, 2)
            });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.OrderId);
        Assert.Equal(25.00m, result.TotalAmount);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.True(unitOfWork.WasSaved);
        Assert.NotNull(repository.Order);
        Assert.Equal(buyerId, repository.Order.BuyerId.Value);
        Assert.Equal(productId, repository.Order.Lines.Single().ProductId);
        Assert.Equal("SKU-1", repository.Order.Lines.Single().ProductSku);
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        public Order? Order { get; private set; }

        public Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            Order = order;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeOrdersUnitOfWork : IOrdersUnitOfWork
    {
        public bool WasSaved { get; private set; }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            WasSaved = true;
            return Task.CompletedTask;
        }
    }
}

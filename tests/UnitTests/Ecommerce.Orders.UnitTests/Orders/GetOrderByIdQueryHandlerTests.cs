using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Application.Orders;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

namespace Ecommerce.Orders.UnitTests.Orders;

public sealed class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldQueryByOrderIdAndBuyerId()
    {
        var repository = new FakeOrderReadRepository();
        var handler = new GetOrderByIdQueryHandler(repository);
        var orderId = Guid.NewGuid();
        var buyerId = Guid.NewGuid();

        await handler.Handle(new GetOrderByIdQuery(orderId, buyerId), CancellationToken.None);

        Assert.Equal(orderId, repository.OrderId);
        Assert.Equal(buyerId, repository.BuyerId);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000000")]
    [InlineData("10000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
    public async Task Handle_ShouldReturnNull_WhenIdsAreEmpty(string orderIdValue, string buyerIdValue)
    {
        var repository = new FakeOrderReadRepository();
        var handler = new GetOrderByIdQueryHandler(repository);

        var result = await handler.Handle(
            new GetOrderByIdQuery(Guid.Parse(orderIdValue), Guid.Parse(buyerIdValue)),
            CancellationToken.None);

        Assert.Null(result);
        Assert.False(repository.WasCalled);
    }

    private sealed class FakeOrderReadRepository : IOrderReadRepository
    {
        public bool WasCalled { get; private set; }

        public Guid OrderId { get; private set; }

        public Guid BuyerId { get; private set; }

        public Task<OrderDetailsDto?> GetByIdForBuyerAsync(
            Guid orderId,
            Guid buyerId,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            OrderId = orderId;
            BuyerId = buyerId;
            return Task.FromResult<OrderDetailsDto?>(null);
        }

        public Task<PagedResult<OrderSummaryDto>> ListForBuyerAsync(
            ListOrdersForBuyerQuery query,
            CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResult<OrderSummaryDto>(
                Array.Empty<OrderSummaryDto>(),
                query.PageNumber ?? ListOrdersForBuyerQueryHandler.DefaultPageNumber,
                query.PageSize ?? ListOrdersForBuyerQueryHandler.DefaultPageSize,
                0));
    }
}

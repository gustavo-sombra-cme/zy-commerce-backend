using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Application.Orders;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

namespace Ecommerce.Orders.UnitTests.Orders;

public sealed class ListOrdersForBuyerQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNullPagination_AppliesDefaults()
    {
        var repository = new FakeOrderReadRepository();
        var handler = new ListOrdersForBuyerQueryHandler(repository);
        var buyerId = Guid.NewGuid();

        await handler.Handle(new ListOrdersForBuyerQuery(buyerId, null, null), CancellationToken.None);

        Assert.NotNull(repository.LastListQuery);
        Assert.Equal(buyerId, repository.LastListQuery.BuyerId);
        Assert.Equal(ListOrdersForBuyerQueryHandler.DefaultPageNumber, repository.LastListQuery.PageNumber);
        Assert.Equal(ListOrdersForBuyerQueryHandler.DefaultPageSize, repository.LastListQuery.PageSize);
    }

    [Fact]
    public async Task Handle_WithExplicitPagination_UsesRequestedValues()
    {
        var repository = new FakeOrderReadRepository();
        var handler = new ListOrdersForBuyerQueryHandler(repository);
        var buyerId = Guid.NewGuid();

        await handler.Handle(new ListOrdersForBuyerQuery(buyerId, 3, 50), CancellationToken.None);

        Assert.NotNull(repository.LastListQuery);
        Assert.Equal(buyerId, repository.LastListQuery.BuyerId);
        Assert.Equal(3, repository.LastListQuery.PageNumber);
        Assert.Equal(50, repository.LastListQuery.PageSize);
    }

    [Fact]
    public async Task Handle_ReturnsRepositoryResult()
    {
        var expected = new PagedResult<OrderSummaryDto>(
            new[]
            {
                new OrderSummaryDto(
                    Guid.NewGuid(),
                    "Created",
                    42.50m,
                    DateTimeOffset.UtcNow,
                    2)
            },
            1,
            20,
            1);
        var repository = new FakeOrderReadRepository(expected);
        var handler = new ListOrdersForBuyerQueryHandler(repository);

        var result = await handler.Handle(
            new ListOrdersForBuyerQuery(Guid.NewGuid(), 1, 20),
            CancellationToken.None);

        Assert.Equal(expected, result);
    }

    private sealed class FakeOrderReadRepository(PagedResult<OrderSummaryDto>? result = null)
        : IOrderReadRepository
    {
        public ListOrdersForBuyerQuery? LastListQuery { get; private set; }

        public Task<OrderDetailsDto?> GetByIdForBuyerAsync(
            Guid orderId,
            Guid buyerId,
            CancellationToken cancellationToken) =>
            Task.FromResult<OrderDetailsDto?>(null);

        public Task<PagedResult<OrderSummaryDto>> ListForBuyerAsync(
            ListOrdersForBuyerQuery query,
            CancellationToken cancellationToken)
        {
            LastListQuery = query;
            return Task.FromResult(result ?? new PagedResult<OrderSummaryDto>(
                Array.Empty<OrderSummaryDto>(),
                query.PageNumber ?? ListOrdersForBuyerQueryHandler.DefaultPageNumber,
                query.PageSize ?? ListOrdersForBuyerQueryHandler.DefaultPageSize,
                0));
        }
    }
}

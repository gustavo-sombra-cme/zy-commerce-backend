using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

namespace Ecommerce.Orders.Application.Orders;

public interface IOrderReadRepository
{
    Task<OrderDetailsDto?> GetByIdForBuyerAsync(
        Guid orderId,
        Guid buyerId,
        CancellationToken cancellationToken);

    Task<PagedResult<OrderSummaryDto>> ListForBuyerAsync(
        ListOrdersForBuyerQuery query,
        CancellationToken cancellationToken);
}

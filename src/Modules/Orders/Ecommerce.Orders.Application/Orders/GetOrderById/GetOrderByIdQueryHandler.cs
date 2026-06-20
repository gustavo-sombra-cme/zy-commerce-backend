using Ecommerce.Orders.Application.Orders;
using MediatR;

namespace Ecommerce.Orders.Application.Orders.GetOrderById;

public sealed class GetOrderByIdQueryHandler(IOrderReadRepository orderReadRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto?>
{
    public Task<OrderDetailsDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.OrderId == Guid.Empty || request.BuyerId == Guid.Empty)
        {
            return Task.FromResult<OrderDetailsDto?>(null);
        }

        return orderReadRepository.GetByIdForBuyerAsync(
            request.OrderId,
            request.BuyerId,
            cancellationToken);
    }
}

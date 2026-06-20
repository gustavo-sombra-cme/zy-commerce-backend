using MediatR;

namespace Ecommerce.Orders.Application.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId, Guid BuyerId) : IRequest<OrderDetailsDto?>;

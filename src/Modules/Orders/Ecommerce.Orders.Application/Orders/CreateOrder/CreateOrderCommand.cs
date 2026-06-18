using MediatR;

namespace Ecommerce.Orders.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    Guid BuyerId,
    IReadOnlyCollection<CreateOrderLineCommand> Lines) : IRequest<CreateOrderResult>;

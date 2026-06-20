using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Application.Orders;
using Ecommerce.Orders.Domain.Orders;
using MediatR;

namespace Ecommerce.Orders.Application.Orders.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrdersUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var buyerId = BuyerId.From(request.BuyerId);
        var lines = request.Lines
            .Select(line => OrderLine.Create(
                line.ProductId,
                line.ProductSku,
                line.ProductName,
                line.UnitPrice,
                line.Quantity))
            .ToArray();

        var order = Order.Create(buyerId, lines, DateTimeOffset.UtcNow);

        await orderRepository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(order.Id.Value, order.TotalAmount, order.CreatedAt);
    }
}

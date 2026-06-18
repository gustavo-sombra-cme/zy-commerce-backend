namespace Ecommerce.Orders.Application.Orders.CreateOrder;

public sealed record CreateOrderResult(
    Guid OrderId,
    decimal TotalAmount,
    DateTimeOffset CreatedAt);

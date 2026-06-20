namespace Ecommerce.Orders.Contracts.Orders;

public sealed record CreateOrderResponse(
    Guid OrderId,
    decimal TotalAmount,
    DateTimeOffset CreatedAt);

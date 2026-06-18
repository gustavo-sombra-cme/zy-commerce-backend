namespace Ecommerce.Orders.Contracts.Orders;

public sealed record OrderLineResponse(
    Guid OrderLineId,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

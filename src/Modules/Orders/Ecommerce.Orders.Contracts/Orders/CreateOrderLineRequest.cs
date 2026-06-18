namespace Ecommerce.Orders.Contracts.Orders;

public sealed record CreateOrderLineRequest(
    Guid ProductId,
    string ProductSku,
    string ProductName,
    decimal UnitPrice,
    int Quantity);

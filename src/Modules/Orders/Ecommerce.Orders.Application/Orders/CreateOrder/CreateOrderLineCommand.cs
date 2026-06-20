namespace Ecommerce.Orders.Application.Orders.CreateOrder;

public sealed record CreateOrderLineCommand(
    Guid ProductId,
    string ProductSku,
    string ProductName,
    decimal UnitPrice,
    int Quantity);

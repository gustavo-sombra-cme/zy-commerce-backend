namespace Ecommerce.Orders.Application.Orders.GetOrderById;

public sealed record OrderLineDetailsDto(
    Guid OrderLineId,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

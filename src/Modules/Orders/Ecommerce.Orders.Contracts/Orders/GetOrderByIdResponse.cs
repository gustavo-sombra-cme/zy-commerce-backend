namespace Ecommerce.Orders.Contracts.Orders;

public sealed record GetOrderByIdResponse(
    Guid OrderId,
    Guid BuyerId,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<OrderLineResponse> Lines);

namespace Ecommerce.Orders.Application.Orders.GetOrderById;

public sealed record OrderDetailsDto(
    Guid OrderId,
    Guid BuyerId,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<OrderLineDetailsDto> Lines);

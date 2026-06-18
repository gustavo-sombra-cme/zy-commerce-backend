namespace Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

public sealed record OrderSummaryDto(
    Guid OrderId,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    int LineCount);

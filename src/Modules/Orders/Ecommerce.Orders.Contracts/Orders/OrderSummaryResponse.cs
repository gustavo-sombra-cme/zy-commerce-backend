namespace Ecommerce.Orders.Contracts.Orders;

public sealed record OrderSummaryResponse(
    Guid OrderId,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    int LineCount);

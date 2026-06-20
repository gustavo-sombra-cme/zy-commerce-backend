namespace Ecommerce.Orders.Contracts.Orders;

public sealed record ListOrdersResponse(
    IReadOnlyCollection<OrderSummaryResponse> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

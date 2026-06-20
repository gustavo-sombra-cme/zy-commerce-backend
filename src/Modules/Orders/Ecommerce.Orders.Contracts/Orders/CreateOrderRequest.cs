namespace Ecommerce.Orders.Contracts.Orders;

public sealed record CreateOrderRequest(IReadOnlyCollection<CreateOrderLineRequest> Lines);

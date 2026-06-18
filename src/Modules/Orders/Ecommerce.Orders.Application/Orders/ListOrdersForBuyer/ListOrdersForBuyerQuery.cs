using Ecommerce.Orders.Application.Abstractions;
using MediatR;

namespace Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

public sealed record ListOrdersForBuyerQuery(
    Guid BuyerId,
    int? PageNumber,
    int? PageSize) : IRequest<PagedResult<OrderSummaryDto>>;

using Ecommerce.Orders.Application.Abstractions;
using MediatR;

namespace Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;

public sealed class ListOrdersForBuyerQueryHandler(IOrderReadRepository orderReadRepository)
    : IRequestHandler<ListOrdersForBuyerQuery, PagedResult<OrderSummaryDto>>
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;

    public Task<PagedResult<OrderSummaryDto>> Handle(
        ListOrdersForBuyerQuery request,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = request with
        {
            PageNumber = request.PageNumber ?? DefaultPageNumber,
            PageSize = request.PageSize ?? DefaultPageSize
        };

        return orderReadRepository.ListForBuyerAsync(normalizedQuery, cancellationToken);
    }
}

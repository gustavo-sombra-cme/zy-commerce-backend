using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Application.Orders;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;
using Ecommerce.Orders.Domain.Orders;
using Ecommerce.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Orders.Infrastructure.Orders;

public sealed class OrderReadRepository(OrdersDbContext dbContext) : IOrderReadRepository
{
    public async Task<OrderDetailsDto?> GetByIdForBuyerAsync(
        Guid orderId,
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        var id = OrderId.From(orderId);
        var ownerId = BuyerId.From(buyerId);

        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Lines)
            .SingleOrDefaultAsync(
                order => order.Id == id && order.BuyerId == ownerId,
                cancellationToken);

        if (order is null)
        {
            return null;
        }

        return new OrderDetailsDto(
            order.Id.Value,
            order.BuyerId.Value,
            order.Status.ToString(),
            order.TotalAmount,
            order.CreatedAt,
            order.Lines
                .Select(line => new OrderLineDetailsDto(
                    line.Id.Value,
                    line.ProductId,
                    line.ProductSku,
                    line.ProductName,
                    line.UnitPrice,
                    line.Quantity,
                    line.LineTotal))
                .ToArray());
    }

    public async Task<PagedResult<OrderSummaryDto>> ListForBuyerAsync(
        ListOrdersForBuyerQuery query,
        CancellationToken cancellationToken)
    {
        var pageNumber = query.PageNumber ?? ListOrdersForBuyerQueryHandler.DefaultPageNumber;
        var pageSize = query.PageSize ?? ListOrdersForBuyerQueryHandler.DefaultPageSize;
        var ownerId = BuyerId.From(query.BuyerId);

        var orders = dbContext.Orders
            .AsNoTracking()
            .Where(order => order.BuyerId == ownerId);

        var totalCount = await orders.CountAsync(cancellationToken);

        var rows = await orders
            .OrderByDescending(order => order.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(order => new
            {
                order.Id,
                order.Status,
                order.TotalAmount,
                order.CreatedAt,
                LineCount = order.Lines.Count
            })
            .ToArrayAsync(cancellationToken);

        var items = rows
            .Select(order => new OrderSummaryDto(
                order.Id.Value,
                order.Status.ToString(),
                order.TotalAmount,
                order.CreatedAt,
                order.LineCount))
            .ToArray();

        return new PagedResult<OrderSummaryDto>(items, pageNumber, pageSize, totalCount);
    }
}

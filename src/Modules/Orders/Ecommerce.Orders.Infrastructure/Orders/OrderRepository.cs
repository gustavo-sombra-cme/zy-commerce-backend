using Ecommerce.Orders.Application.Orders;
using Ecommerce.Orders.Domain.Orders;
using Ecommerce.Orders.Infrastructure.Persistence;

namespace Ecommerce.Orders.Infrastructure.Orders;

public sealed class OrderRepository(OrdersDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken) =>
        await dbContext.Orders.AddAsync(order, cancellationToken);
}

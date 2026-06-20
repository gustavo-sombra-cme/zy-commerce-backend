using Ecommerce.Orders.Domain.Orders;

namespace Ecommerce.Orders.Application.Orders;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
}

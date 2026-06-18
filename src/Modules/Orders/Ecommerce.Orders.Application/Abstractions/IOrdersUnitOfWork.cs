namespace Ecommerce.Orders.Application.Abstractions;

public interface IOrdersUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

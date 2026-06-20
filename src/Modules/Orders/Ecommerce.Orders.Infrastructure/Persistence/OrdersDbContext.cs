using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Orders.Infrastructure.Persistence;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options)
    : DbContext(options), IOrdersUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();

    async Task IOrdersUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        await SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
    }
}

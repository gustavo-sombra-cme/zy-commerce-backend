using Ecommerce.Orders.Application.Abstractions;
using Ecommerce.Orders.Application.Orders;
using Ecommerce.Orders.Infrastructure.Orders;
using Ecommerce.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Orders.Infrastructure.DependencyInjection;

public static class OrdersInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddOrdersInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Orders connection string is required.", nameof(connectionString));
        }

        services.AddDbContext<OrdersDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderReadRepository, OrderReadRepository>();
        services.AddScoped<IOrdersUnitOfWork>(provider => provider.GetRequiredService<OrdersDbContext>());

        return services;
    }
}

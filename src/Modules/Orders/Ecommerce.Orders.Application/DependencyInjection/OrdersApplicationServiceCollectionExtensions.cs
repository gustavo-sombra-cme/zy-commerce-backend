using Ecommerce.Orders.Application.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Orders.Application.DependencyInjection;

public static class OrdersApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(OrdersApplicationServiceCollectionExtensions).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(OrdersApplicationServiceCollectionExtensions).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

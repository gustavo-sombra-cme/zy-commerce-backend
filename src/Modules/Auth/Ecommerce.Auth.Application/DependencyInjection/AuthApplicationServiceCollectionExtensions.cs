using Ecommerce.Auth.Application.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Auth.Application.DependencyInjection;

public static class AuthApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(AuthApplicationServiceCollectionExtensions).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(AuthApplicationServiceCollectionExtensions).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

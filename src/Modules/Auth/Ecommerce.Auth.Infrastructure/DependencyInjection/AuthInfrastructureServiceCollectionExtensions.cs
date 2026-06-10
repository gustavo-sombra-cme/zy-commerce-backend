using Ecommerce.Auth.Application.Abstractions;
using Ecommerce.Auth.Application.Security;
using Ecommerce.Auth.Application.Users;
using Ecommerce.Auth.Infrastructure.Persistence;
using Ecommerce.Auth.Infrastructure.Security;
using Ecommerce.Auth.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Auth.Infrastructure.DependencyInjection;

public static class AuthInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services,
        string connectionString,
        JwtOptions jwtOptions)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Auth connection string is required.", nameof(connectionString));
        }

        services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthUnitOfWork>(provider => provider.GetRequiredService<AuthDbContext>());
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton(jwtOptions);
        services.AddScoped<IAccessTokenGenerator, JwtAccessTokenGenerator>();

        return services;
    }
}

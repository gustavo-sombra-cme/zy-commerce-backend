using Ecommerce.Auth.Application.Security;
using Ecommerce.Auth.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Auth.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string Hash(string password) =>
        _passwordHasher.HashPassword(new object(), password);

    public bool Verify(string password, PasswordHash passwordHash) =>
        _passwordHasher.VerifyHashedPassword(new object(), passwordHash.Value, password)
            != PasswordVerificationResult.Failed;
}

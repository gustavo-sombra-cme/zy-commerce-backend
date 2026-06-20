using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.Application.Security;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, PasswordHash passwordHash);
}

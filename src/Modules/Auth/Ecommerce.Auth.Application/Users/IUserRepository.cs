using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.Application.Users;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken) =>
        throw new NotSupportedException("User lookup by email is not implemented by this repository.");

    Task AddAsync(User user, CancellationToken cancellationToken);
}

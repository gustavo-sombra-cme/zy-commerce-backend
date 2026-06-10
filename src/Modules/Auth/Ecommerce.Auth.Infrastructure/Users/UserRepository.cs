using Ecommerce.Auth.Application.Users;
using Ecommerce.Auth.Domain.Users;
using Ecommerce.Auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Auth.Infrastructure.Users;

public sealed class UserRepository(AuthDbContext dbContext) : IUserRepository
{
    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken) =>
        await dbContext.Users.AddAsync(user, cancellationToken);
}

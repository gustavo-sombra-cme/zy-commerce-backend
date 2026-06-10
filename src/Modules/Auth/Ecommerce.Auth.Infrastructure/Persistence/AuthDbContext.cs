using Ecommerce.Auth.Application.Abstractions;
using Ecommerce.Auth.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Auth.Infrastructure.Persistence;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options)
    : DbContext(options), IAuthUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    async Task IAuthUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        await SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auth");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
    }
}

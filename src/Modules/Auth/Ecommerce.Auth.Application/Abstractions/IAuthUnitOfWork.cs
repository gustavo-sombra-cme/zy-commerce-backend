namespace Ecommerce.Auth.Application.Abstractions;

public interface IAuthUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

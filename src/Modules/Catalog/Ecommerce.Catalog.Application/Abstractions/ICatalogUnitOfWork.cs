namespace Ecommerce.Catalog.Application.Abstractions;

public interface ICatalogUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

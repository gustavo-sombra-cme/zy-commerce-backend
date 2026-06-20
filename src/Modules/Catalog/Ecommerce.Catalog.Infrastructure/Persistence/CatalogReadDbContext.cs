using Ecommerce.Catalog.Domain.Products;
using Ecommerce.Catalog.Infrastructure.Products;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Catalog.Infrastructure.Persistence;

public sealed class CatalogReadDbContext(DbContextOptions<CatalogReadDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.Entity<ProductSearchReadModel>(builder =>
        {
            builder.HasNoKey();

            builder.ToTable("Products", "catalog", table => table.ExcludeFromMigrations());

            builder.Property(product => product.Id)
                .HasColumnName("Id");

            builder.Property(product => product.Sku)
                .HasColumnName("Sku")
                .HasMaxLength(Sku.MaxLength)
                .IsRequired();

            builder.Property(product => product.Name)
                .HasColumnName("Name")
                .HasMaxLength(ProductName.MaxLength)
                .IsRequired();

            builder.Property(product => product.Description)
                .HasColumnName("Description")
                .HasMaxLength(Product.DescriptionMaxLength);

            builder.Property(product => product.Price)
                .HasColumnName("Price")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(product => product.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            builder.Property(product => product.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            builder.Property(product => product.UpdatedAt)
                .HasColumnName("UpdatedAt");
        });
    }
}

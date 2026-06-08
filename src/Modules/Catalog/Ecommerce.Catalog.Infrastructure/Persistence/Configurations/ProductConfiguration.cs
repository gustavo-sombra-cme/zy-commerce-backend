using Ecommerce.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Catalog.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .HasConversion(
                productId => productId.Value,
                value => ProductId.From(value))
            .ValueGeneratedNever();

        builder.Property(product => product.Sku)
            .HasConversion(
                sku => sku.Value,
                value => Sku.Create(value))
            .HasColumnName("Sku")
            .HasMaxLength(Sku.MaxLength)
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique();

        builder.Property(product => product.Name)
            .HasConversion(
                name => name.Value,
                value => ProductName.Create(value))
            .HasColumnName("Name")
            .HasMaxLength(ProductName.MaxLength)
            .IsRequired();

        builder.Property(product => product.Description)
            .HasMaxLength(Product.DescriptionMaxLength);

        builder.Property(product => product.IsActive)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.Property(product => product.UpdatedAt);
    }
}

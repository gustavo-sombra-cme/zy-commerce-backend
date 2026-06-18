using Ecommerce.Orders.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Orders.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .HasConversion(
                orderId => orderId.Value,
                value => OrderId.From(value))
            .ValueGeneratedNever();

        builder.Property(order => order.BuyerId)
            .HasConversion(
                buyerId => buyerId.Value,
                value => BuyerId.From(value))
            .IsRequired();

        builder.HasIndex(order => order.BuyerId);

        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasMany(order => order.Lines)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

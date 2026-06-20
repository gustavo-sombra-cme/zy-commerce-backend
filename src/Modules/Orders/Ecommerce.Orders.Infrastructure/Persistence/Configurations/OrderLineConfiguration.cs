using Ecommerce.Orders.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Orders.Infrastructure.Persistence.Configurations;

public sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("OrderLines");

        builder.HasKey(orderLine => orderLine.Id);

        builder.Property<OrderId>("OrderId")
            .HasConversion(
                orderId => orderId.Value,
                value => OrderId.From(value))
            .IsRequired();

        builder.Property(orderLine => orderLine.Id)
            .HasConversion(
                orderLineId => orderLineId.Value,
                value => OrderLineId.From(value))
            .ValueGeneratedNever();

        builder.Property(orderLine => orderLine.ProductId)
            .IsRequired();

        builder.Property(orderLine => orderLine.ProductSku)
            .HasMaxLength(OrderLine.ProductSkuMaxLength)
            .IsRequired();

        builder.Property(orderLine => orderLine.ProductName)
            .HasMaxLength(OrderLine.ProductNameMaxLength)
            .IsRequired();

        builder.Property(orderLine => orderLine.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(orderLine => orderLine.Quantity)
            .IsRequired();

        builder.Ignore(orderLine => orderLine.LineTotal);
    }
}

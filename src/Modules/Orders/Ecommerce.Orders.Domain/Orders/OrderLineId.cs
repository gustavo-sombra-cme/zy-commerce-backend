namespace Ecommerce.Orders.Domain.Orders;

public readonly record struct OrderLineId(Guid Value)
{
    public static OrderLineId New() => new(Guid.NewGuid());

    public static OrderLineId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Order line id cannot be empty.", nameof(value));
        }

        return new OrderLineId(value);
    }
}

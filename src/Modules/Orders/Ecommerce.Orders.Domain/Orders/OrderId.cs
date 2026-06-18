namespace Ecommerce.Orders.Domain.Orders;

public readonly record struct OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());

    public static OrderId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Order id cannot be empty.", nameof(value));
        }

        return new OrderId(value);
    }
}

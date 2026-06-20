namespace Ecommerce.Orders.Domain.Orders;

public readonly record struct BuyerId(Guid Value)
{
    public static BuyerId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Buyer id cannot be empty.", nameof(value));
        }

        return new BuyerId(value);
    }
}

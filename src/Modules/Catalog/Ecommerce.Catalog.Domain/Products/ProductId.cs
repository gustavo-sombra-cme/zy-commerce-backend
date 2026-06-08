namespace Ecommerce.Catalog.Domain.Products;

public readonly record struct ProductId(Guid Value)
{
    public static ProductId New() => new(Guid.NewGuid());

    public static ProductId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(value));
        }

        return new ProductId(value);
    }
}

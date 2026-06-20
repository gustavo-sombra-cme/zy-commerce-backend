namespace Ecommerce.Orders.Domain.Orders;

public sealed class OrderLine
{
    public const int ProductSkuMaxLength = 64;
    public const int ProductNameMaxLength = 200;

    private OrderLine()
    {
    }

    private OrderLine(
        OrderLineId id,
        Guid productId,
        string productSku,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        Id = id;
        ProductId = productId;
        ProductSku = NormalizeRequired(productSku, ProductSkuMaxLength, nameof(productSku));
        ProductName = NormalizeRequired(productName, ProductNameMaxLength, nameof(productName));
        UnitPrice = ValidateUnitPrice(unitPrice);
        Quantity = ValidateQuantity(quantity);
    }

    public OrderLineId Id { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductSku { get; private set; } = string.Empty;

    public string ProductName { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal LineTotal => UnitPrice * Quantity;

    public static OrderLine Create(
        Guid productId,
        string productSku,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("Product id cannot be empty.", nameof(productId));
        }

        return new OrderLine(
            OrderLineId.New(),
            productId,
            productSku,
            productName,
            unitPrice,
            quantity);
    }

    private static decimal ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        }

        return decimal.Round(unitPrice, 2, MidpointRounding.AwayFromZero);
    }

    private static int ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        return quantity;
    }

    private static string NormalizeRequired(string value, int maxLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        var trimmed = value.Trim();

        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"Value cannot exceed {maxLength} characters.", parameterName);
        }

        return trimmed;
    }
}

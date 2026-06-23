namespace Ecommerce.Catalog.Domain.Products;

public sealed class Product
{
    public const int DescriptionMaxLength = 2000;

    private Product()
    {
    }

    private Product(ProductId id, Sku sku, ProductName name, string? description, decimal price, DateTimeOffset createdAt)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Description = NormalizeDescription(description);
        Price = NormalizePrice(price);
        IsActive = true;
        CreatedAt = createdAt;
    }

    public ProductId Id { get; private set; }

    public Sku Sku { get; private set; }

    public ProductName Name { get; private set; }

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Product Create(
        Sku sku,
        ProductName name,
        string? description,
        decimal price,
        DateTimeOffset createdAt) =>
        new(ProductId.New(), sku, name, description, price, createdAt);

    public static Product Create(Sku sku, ProductName name, string? description, DateTimeOffset createdAt) =>
        Create(sku, name, description, 0, createdAt);

    public void Deactivate(DateTimeOffset updatedAt)
    {
        if (updatedAt == default)
        {
            throw new ArgumentException("Updated timestamp is required.", nameof(updatedAt));
        }

        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAt = updatedAt;
    }

    public void Reactivate(DateTimeOffset updatedAt)
    {
        if (IsActive)
        {
            return;
        }

        if (updatedAt == default)
        {
            throw new ArgumentException("Updated timestamp is required.", nameof(updatedAt));
        }

        IsActive = true;
        UpdatedAt = updatedAt;
    }

    public void UpdateDetails(ProductName name, string? description, DateTimeOffset updatedAt)
    {
        if (updatedAt == default)
        {
            throw new ArgumentException("Updated timestamp is required.", nameof(updatedAt));
        }

        Name = name;
        Description = NormalizeDescription(description);
        UpdatedAt = updatedAt;
    }

    public void UpdatePrice(decimal price, DateTimeOffset updatedAt)
    {
        if (updatedAt == default)
        {
            throw new ArgumentException("Updated timestamp is required.", nameof(updatedAt));
        }

        Price = NormalizePrice(price);
        UpdatedAt = updatedAt;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var trimmed = description.Trim();

        if (trimmed.Length > DescriptionMaxLength)
        {
            throw new ArgumentException($"Description cannot exceed {DescriptionMaxLength} characters.", nameof(description));
        }

        return trimmed;
    }

    private static decimal NormalizePrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        }

        return decimal.Round(price, 2, MidpointRounding.AwayFromZero);
    }
}

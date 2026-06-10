namespace Ecommerce.Catalog.Domain.Products;

public sealed class Product
{
    public const int DescriptionMaxLength = 2000;

    private Product()
    {
    }

    private Product(ProductId id, Sku sku, ProductName name, string? description, DateTimeOffset createdAt)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Description = NormalizeDescription(description);
        IsActive = true;
        CreatedAt = createdAt;
    }

    public ProductId Id { get; private set; }

    public Sku Sku { get; private set; }

    public ProductName Name { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Product Create(Sku sku, ProductName name, string? description, DateTimeOffset createdAt) =>
        new(ProductId.New(), sku, name, description, createdAt);

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
}

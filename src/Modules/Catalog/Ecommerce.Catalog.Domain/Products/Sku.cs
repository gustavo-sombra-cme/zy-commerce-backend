namespace Ecommerce.Catalog.Domain.Products;

public readonly record struct Sku
{
    public const int MaxLength = 64;

    private Sku(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Sku Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("SKU is required.", nameof(value));
        }

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length > MaxLength)
        {
            throw new ArgumentException($"SKU cannot exceed {MaxLength} characters.", nameof(value));
        }

        if (!normalized.All(IsAllowedCharacter))
        {
            throw new ArgumentException("SKU can contain only uppercase letters, numbers, hyphen, or underscore.", nameof(value));
        }

        return new Sku(normalized);
    }

    public override string ToString() => Value;

    private static bool IsAllowedCharacter(char value) =>
        value is >= 'A' and <= 'Z'
            or >= '0' and <= '9'
            or '-'
            or '_';
}

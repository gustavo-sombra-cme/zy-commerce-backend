namespace Ecommerce.Auth.Domain.Users;

public readonly record struct Email
{
    public const int MaxLength = 320;

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email is required.", nameof(value));
        }

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > MaxLength)
        {
            throw new ArgumentException($"Email cannot exceed {MaxLength} characters.", nameof(value));
        }

        if (!HasValidShape(normalized))
        {
            throw new ArgumentException("Email is invalid.", nameof(value));
        }

        return new Email(normalized);
    }

    public override string ToString() => Value;

    private static bool HasValidShape(string value)
    {
        var atIndex = value.IndexOf('@', StringComparison.Ordinal);
        var lastAtIndex = value.LastIndexOf('@');

        if (atIndex <= 0 || atIndex != lastAtIndex || atIndex == value.Length - 1)
        {
            return false;
        }

        var domain = value[(atIndex + 1)..];
        var dotIndex = domain.LastIndexOf('.');

        return dotIndex > 0
            && dotIndex < domain.Length - 1
            && !value.Contains(' ', StringComparison.Ordinal);
    }
}

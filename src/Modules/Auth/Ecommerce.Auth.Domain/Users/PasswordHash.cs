namespace Ecommerce.Auth.Domain.Users;

public readonly record struct PasswordHash
{
    private PasswordHash(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static PasswordHash Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Password hash is required.", nameof(value));
        }

        return new PasswordHash(value.Trim());
    }

    public override string ToString() => Value;
}

namespace Ecommerce.Auth.Domain.Users;

public readonly record struct UserId
{
    private UserId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static UserId New() => new(Guid.NewGuid());

    public static UserId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(value));
        }

        return new UserId(value);
    }

    public override string ToString() => Value.ToString();
}

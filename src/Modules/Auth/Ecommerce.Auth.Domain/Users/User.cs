namespace Ecommerce.Auth.Domain.Users;

public sealed class User
{
    private User()
    {
    }

    private User(UserId id, Email email, PasswordHash passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
        IsEmailVerified = false;
        CreatedAt = createdAt;
    }

    public UserId Id { get; private set; }

    public Email Email { get; private set; }

    public PasswordHash PasswordHash { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsEmailVerified { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static User Register(UserId id, Email email, PasswordHash passwordHash, DateTimeOffset createdAt)
    {
        if (id.Value == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(id));
        }

        if (createdAt == default)
        {
            throw new ArgumentException("Created timestamp is required.", nameof(createdAt));
        }

        return new User(id, email, passwordHash, createdAt);
    }

    public void VerifyEmail(DateTimeOffset updatedAt)
    {
        EnsureUpdatedAt(updatedAt);

        if (IsEmailVerified)
        {
            return;
        }

        IsEmailVerified = true;
        UpdatedAt = updatedAt;
    }

    public void ChangePassword(PasswordHash passwordHash, DateTimeOffset updatedAt)
    {
        EnsureUpdatedAt(updatedAt);

        PasswordHash = passwordHash;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        EnsureUpdatedAt(updatedAt);

        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAt = updatedAt;
    }

    public void Reactivate(DateTimeOffset updatedAt)
    {
        EnsureUpdatedAt(updatedAt);

        if (IsActive)
        {
            return;
        }

        IsActive = true;
        UpdatedAt = updatedAt;
    }

    private static void EnsureUpdatedAt(DateTimeOffset updatedAt)
    {
        if (updatedAt == default)
        {
            throw new ArgumentException("Updated timestamp is required.", nameof(updatedAt));
        }
    }
}

using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class UserTests
{
    [Fact]
    public void Register_WithValidValues_CreatesActiveUnverifiedUser()
    {
        var userId = UserId.New();
        var email = Email.Create("user@example.com");
        var passwordHash = PasswordHash.Create("hash-value");
        var createdAt = DateTimeOffset.UtcNow;

        var user = User.Register(userId, email, passwordHash, createdAt);

        Assert.Equal(userId, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(UserRole.Customer, user.Role);
        Assert.True(user.IsActive);
        Assert.False(user.IsEmailVerified);
        Assert.Equal(createdAt, user.CreatedAt);
        Assert.Null(user.UpdatedAt);
    }

    [Fact]
    public void Register_WithAdminRole_CreatesAdminUser()
    {
        var user = User.Register(
            UserId.New(),
            Email.Create("admin@example.com"),
            PasswordHash.Create("hash-value"),
            UserRole.Admin,
            DateTimeOffset.UtcNow);

        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public void Register_WithInvalidRole_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => User.Register(
            UserId.New(),
            Email.Create("user@example.com"),
            PasswordHash.Create("hash-value"),
            (UserRole)999,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Register_WithEmptyUserId_Throws()
    {
        Assert.Throws<ArgumentException>(() => User.Register(
            default,
            Email.Create("user@example.com"),
            PasswordHash.Create("hash-value"),
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Register_WithDefaultCreatedAt_Throws()
    {
        Assert.Throws<ArgumentException>(() => User.Register(
            UserId.New(),
            Email.Create("user@example.com"),
            PasswordHash.Create("hash-value"),
            default));
    }

    [Fact]
    public void VerifyEmail_WithUnverifiedUser_SetsVerifiedAndUpdatedAt()
    {
        var user = CreateUser();
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        user.VerifyEmail(updatedAt);

        Assert.True(user.IsEmailVerified);
        Assert.Equal(updatedAt, user.UpdatedAt);
    }

    [Fact]
    public void VerifyEmail_WithVerifiedUser_IsIdempotent()
    {
        var user = CreateUser();
        var firstUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(1);
        var secondUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        user.VerifyEmail(firstUpdatedAt);
        user.VerifyEmail(secondUpdatedAt);

        Assert.True(user.IsEmailVerified);
        Assert.Equal(firstUpdatedAt, user.UpdatedAt);
    }

    [Fact]
    public void ChangePassword_WithValidHash_UpdatesPasswordHashAndUpdatedAt()
    {
        var user = CreateUser();
        var newPasswordHash = PasswordHash.Create("new-hash-value");
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        user.ChangePassword(newPasswordHash, updatedAt);

        Assert.Equal(newPasswordHash, user.PasswordHash);
        Assert.Equal(updatedAt, user.UpdatedAt);
    }

    [Fact]
    public void Deactivate_WithActiveUser_SetsInactiveAndUpdatedAt()
    {
        var user = CreateUser();
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        user.Deactivate(updatedAt);

        Assert.False(user.IsActive);
        Assert.Equal(updatedAt, user.UpdatedAt);
    }

    [Fact]
    public void Deactivate_WithInactiveUser_IsIdempotent()
    {
        var user = CreateUser();
        var firstUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(1);
        var secondUpdatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        user.Deactivate(firstUpdatedAt);
        user.Deactivate(secondUpdatedAt);

        Assert.False(user.IsActive);
        Assert.Equal(firstUpdatedAt, user.UpdatedAt);
    }

    [Fact]
    public void Reactivate_WithInactiveUser_SetsActiveAndUpdatedAt()
    {
        var user = CreateUser();
        var deactivatedAt = DateTimeOffset.UtcNow.AddMinutes(1);
        var reactivatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        user.Deactivate(deactivatedAt);
        user.Reactivate(reactivatedAt);

        Assert.True(user.IsActive);
        Assert.Equal(reactivatedAt, user.UpdatedAt);
    }

    [Fact]
    public void Reactivate_WithActiveUser_IsIdempotent()
    {
        var user = CreateUser();
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        user.Reactivate(updatedAt);

        Assert.True(user.IsActive);
        Assert.Null(user.UpdatedAt);
    }

    [Theory]
    [InlineData("VerifyEmail")]
    [InlineData("ChangePassword")]
    [InlineData("Deactivate")]
    [InlineData("Reactivate")]
    public void StateTransition_WithDefaultUpdatedAt_Throws(string behavior)
    {
        var user = CreateUser();

        Assert.Throws<ArgumentException>(() =>
        {
            switch (behavior)
            {
                case "VerifyEmail":
                    user.VerifyEmail(default);
                    break;
                case "ChangePassword":
                    user.ChangePassword(PasswordHash.Create("new-hash"), default);
                    break;
                case "Deactivate":
                    user.Deactivate(default);
                    break;
                case "Reactivate":
                    user.Reactivate(default);
                    break;
            }
        });
    }

    private static User CreateUser() =>
        User.Register(
            UserId.New(),
            Email.Create("user@example.com"),
            PasswordHash.Create("hash-value"),
            DateTimeOffset.UtcNow);
}

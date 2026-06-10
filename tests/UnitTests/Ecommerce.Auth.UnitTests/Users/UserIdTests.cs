using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class UserIdTests
{
    [Fact]
    public void New_CreatesNonEmptyUserId()
    {
        var userId = UserId.New();

        Assert.NotEqual(Guid.Empty, userId.Value);
    }

    [Fact]
    public void From_WithNonEmptyGuid_CreatesUserId()
    {
        var value = Guid.NewGuid();

        var userId = UserId.From(value);

        Assert.Equal(value, userId.Value);
    }

    [Fact]
    public void From_WithEmptyGuid_Throws()
    {
        Assert.Throws<ArgumentException>(() => UserId.From(Guid.Empty));
    }
}

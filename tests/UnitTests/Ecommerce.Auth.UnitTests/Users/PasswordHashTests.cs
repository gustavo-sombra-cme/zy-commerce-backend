using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class PasswordHashTests
{
    [Fact]
    public void Create_WithValidHash_CreatesPasswordHash()
    {
        var passwordHash = PasswordHash.Create("hash-value");

        Assert.Equal("hash-value", passwordHash.Value);
    }

    [Fact]
    public void Create_WithWhitespace_TrimsHash()
    {
        var passwordHash = PasswordHash.Create("  hash-value  ");

        Assert.Equal("hash-value", passwordHash.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidHash_Throws(string value)
    {
        Assert.Throws<ArgumentException>(() => PasswordHash.Create(value));
    }
}

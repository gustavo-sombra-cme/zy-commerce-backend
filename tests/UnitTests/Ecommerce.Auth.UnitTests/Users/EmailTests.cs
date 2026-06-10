using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class EmailTests
{
    [Theory]
    [InlineData("USER@example.COM", "user@example.com")]
    [InlineData("  user@example.com  ", "user@example.com")]
    public void Create_WithValidEmail_NormalizesValue(string input, string expected)
    {
        var email = Email.Create(input);

        Assert.Equal(expected, email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("user")]
    [InlineData("user@")]
    [InlineData("@example.com")]
    [InlineData("user@example")]
    [InlineData("user@@example.com")]
    [InlineData("user name@example.com")]
    public void Create_WithInvalidEmail_Throws(string value)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(value));
    }

    [Fact]
    public void Create_WithOverlongEmail_Throws()
    {
        var email = $"{new string('a', Email.MaxLength)}@example.com";

        Assert.Throws<ArgumentException>(() => Email.Create(email));
    }
}

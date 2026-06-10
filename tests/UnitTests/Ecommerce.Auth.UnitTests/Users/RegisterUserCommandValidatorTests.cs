using Ecommerce.Auth.Application.Users.RegisterUser;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Succeeds()
    {
        var result = _validator.Validate(new RegisterUserCommand("user@example.com", "Password123"));

        Assert.True(result.IsValid);
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
    public void Validate_WithInvalidEmail_Fails(string email)
    {
        var result = _validator.Validate(new RegisterUserCommand(email, "Password123"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserCommand.Email));
    }

    [Fact]
    public void Validate_WithOverlongEmail_Fails()
    {
        var email = $"{new string('a', EmailMaxLength)}@example.com";

        var result = _validator.Validate(new RegisterUserCommand(email, "Password123"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("short")]
    public void Validate_WithInvalidPassword_Fails(string password)
    {
        var result = _validator.Validate(new RegisterUserCommand("user@example.com", password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserCommand.Password));
    }

    [Fact]
    public void Validate_WithOverlongPassword_Fails()
    {
        var password = new string('A', RegisterUserCommandValidator.PasswordMaxLength + 1);

        var result = _validator.Validate(new RegisterUserCommand("user@example.com", password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterUserCommand.Password));
    }

    private const int EmailMaxLength = 320;
}

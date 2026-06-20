namespace Ecommerce.Auth.Application.Users.LoginUser;

public sealed class InvalidCredentialsException()
    : Exception("Invalid email or password.");

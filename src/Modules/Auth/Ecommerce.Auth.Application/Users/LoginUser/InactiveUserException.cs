namespace Ecommerce.Auth.Application.Users.LoginUser;

public sealed class InactiveUserException(string email)
    : Exception($"User '{email}' is inactive.")
{
    public string Email { get; } = email;
}

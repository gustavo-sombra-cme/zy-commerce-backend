namespace Ecommerce.Auth.Application.Users.RegisterUser;

public sealed class DuplicateEmailException(string email)
    : Exception($"A user with email '{email}' already exists.")
{
    public string Email { get; } = email;
}

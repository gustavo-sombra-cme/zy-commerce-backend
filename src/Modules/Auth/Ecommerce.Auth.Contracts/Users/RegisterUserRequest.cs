namespace Ecommerce.Auth.Contracts.Users;

public sealed record RegisterUserRequest(
    string Email,
    string Password);

namespace Ecommerce.Auth.Application.Users.RegisterUser;

public sealed record RegisterUserResult(
    Guid UserId,
    string Email);

namespace Ecommerce.Auth.Application.Users.LoginUser;

public sealed record LoginUserResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt);

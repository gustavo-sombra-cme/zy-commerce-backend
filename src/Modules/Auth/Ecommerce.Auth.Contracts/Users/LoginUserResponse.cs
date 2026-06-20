namespace Ecommerce.Auth.Contracts.Users;

public sealed record LoginUserResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt);

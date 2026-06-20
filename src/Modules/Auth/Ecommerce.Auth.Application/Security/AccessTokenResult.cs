namespace Ecommerce.Auth.Application.Security;

public sealed record AccessTokenResult(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt);

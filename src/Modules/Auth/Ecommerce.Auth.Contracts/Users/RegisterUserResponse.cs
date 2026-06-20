namespace Ecommerce.Auth.Contracts.Users;

public sealed record RegisterUserResponse(
    Guid UserId,
    string Email);

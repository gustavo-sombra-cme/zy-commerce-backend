namespace Ecommerce.Auth.Contracts.Users;

public sealed record GetCurrentUserResponse(
    Guid UserId,
    string Email,
    string Role);

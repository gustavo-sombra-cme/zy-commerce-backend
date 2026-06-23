using Ecommerce.Auth.Application.Users.LoginUser;
using Ecommerce.Auth.Application.Users.RegisterUser;
using Ecommerce.Auth.Contracts.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Auth;

[ApiController]
[Route("api/auth/users")]
public sealed class AuthUsersController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResponse>> RegisterUser(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterUserCommand(request.Email, request.Password),
            cancellationToken);

        var response = new RegisterUserResponse(result.UserId, result.Email);

        return Created($"/api/auth/users/{response.UserId}", response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginUserResponse>> LoginUser(
        LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginUserCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(new LoginUserResponse(
            result.UserId,
            result.Email,
            result.AccessToken,
            result.TokenType,
            result.ExpiresAt));
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(GetCurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<GetCurrentUserResponse> GetCurrentUser()
    {
        var subject = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("role")?.Value;

        if (!Guid.TryParse(subject, out var userId)
            || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(role))
        {
            return Unauthorized();
        }

        return Ok(new GetCurrentUserResponse(userId, email, role));
    }
}

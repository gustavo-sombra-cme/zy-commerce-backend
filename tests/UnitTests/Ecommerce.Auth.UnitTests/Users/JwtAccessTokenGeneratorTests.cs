using System.IdentityModel.Tokens.Jwt;
using Ecommerce.Auth.Domain.Users;
using Ecommerce.Auth.Infrastructure.Security;

namespace Ecommerce.Auth.UnitTests.Users;

public sealed class JwtAccessTokenGeneratorTests
{
    [Fact]
    public void Generate_WithAdminUser_IncludesRoleClaim()
    {
        var user = User.Register(
            UserId.New(),
            Email.Create("admin@example.com"),
            PasswordHash.Create("hash-value"),
            UserRole.Admin,
            DateTimeOffset.UtcNow);
        var generator = new JwtAccessTokenGenerator(new JwtOptions
        {
            Issuer = "Ecommerce.Api",
            Audience = "Ecommerce.Api",
            SigningKey = "test-signing-key-that-is-long-enough",
            AccessTokenLifetimeMinutes = 15
        });

        var result = generator.Generate(user);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        Assert.Contains(token.Claims, claim => claim.Type == "role" && claim.Value == "Admin");
    }
}

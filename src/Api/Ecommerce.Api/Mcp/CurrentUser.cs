using System.Security.Claims;

namespace Ecommerce.Api.Mcp;

internal static class CurrentUser
{
    public static bool TryGetUserId(ClaimsPrincipal? user, out Guid userId)
    {
        var subject = user?.FindFirst("sub")?.Value;
        return Guid.TryParse(subject, out userId);
    }
}

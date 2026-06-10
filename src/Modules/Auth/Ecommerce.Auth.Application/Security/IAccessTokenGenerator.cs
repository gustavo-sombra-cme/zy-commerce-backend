using Ecommerce.Auth.Domain.Users;

namespace Ecommerce.Auth.Application.Security;

public interface IAccessTokenGenerator
{
    AccessTokenResult Generate(User user);
}

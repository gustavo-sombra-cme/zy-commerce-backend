using System.Reflection;
using Ecommerce.Api.Controllers.Auth;
using Ecommerce.Api.Controllers.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.ArchitectureTests;

public sealed class ApiAuthorizationTests
{
    [Theory]
    [InlineData(nameof(ProductsController.CreateProduct))]
    [InlineData(nameof(ProductsController.UpdateProductDetails))]
    [InlineData(nameof(ProductsController.DeactivateProduct))]
    public void CatalogWriteEndpoints_ShouldRequireAuthorization(string actionName)
    {
        var method = GetAction(typeof(ProductsController), actionName);

        Assert.True(HasAuthorizeAttribute(method), $"{actionName} should require authorization.");
    }

    [Theory]
    [InlineData(nameof(ProductsController.SearchProducts))]
    [InlineData(nameof(ProductsController.GetProductById))]
    public void CatalogReadEndpoints_ShouldRemainPublic(string actionName)
    {
        var method = GetAction(typeof(ProductsController), actionName);

        Assert.False(HasAuthorizeAttribute(method), $"{actionName} should remain public.");
    }

    [Theory]
    [InlineData(nameof(AuthUsersController.RegisterUser))]
    [InlineData(nameof(AuthUsersController.LoginUser))]
    public void AuthRegisterAndLoginEndpoints_ShouldRemainPublic(string actionName)
    {
        var method = GetAction(typeof(AuthUsersController), actionName);

        Assert.False(HasAuthorizeAttribute(method), $"{actionName} should remain public.");
    }

    [Fact]
    public void AuthCurrentUserEndpoint_ShouldRequireAuthorization()
    {
        var method = GetAction(typeof(AuthUsersController), nameof(AuthUsersController.GetCurrentUser));

        Assert.True(HasAuthorizeAttribute(method), "GetCurrentUser should require authorization.");
    }

    [Theory]
    [InlineData(nameof(ProductsController.CreateProduct), typeof(HttpPostAttribute))]
    [InlineData(nameof(ProductsController.UpdateProductDetails), typeof(HttpPutAttribute))]
    [InlineData(nameof(ProductsController.DeactivateProduct), typeof(HttpDeleteAttribute))]
    public void CatalogProtectedEndpoints_ShouldUseExpectedHttpMethods(string actionName, Type attributeType)
    {
        var method = GetAction(typeof(ProductsController), actionName);

        Assert.Contains(method.GetCustomAttributes(), attribute => attribute.GetType() == attributeType);
    }

    private static MethodInfo GetAction(Type controllerType, string actionName)
    {
        return controllerType.GetMethod(actionName)
            ?? throw new InvalidOperationException($"Could not find {controllerType.Name}.{actionName}.");
    }

    private static bool HasAuthorizeAttribute(MethodInfo method)
    {
        return method.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Any()
            || method.DeclaringType?.GetCustomAttributes<AuthorizeAttribute>(inherit: true).Any() == true;
    }
}

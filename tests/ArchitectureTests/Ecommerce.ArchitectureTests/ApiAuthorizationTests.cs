using System.Reflection;
using Ecommerce.Api.Controllers.Auth;
using Ecommerce.Api.Controllers.Assistant;
using Ecommerce.Api.Controllers.Catalog;
using Ecommerce.Api.Controllers.Orders;
using Ecommerce.Orders.Contracts.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.ArchitectureTests;

public sealed class ApiAuthorizationTests
{
    [Theory]
    [InlineData(nameof(ProductsController.CreateProduct))]
    [InlineData(nameof(ProductsController.UpdateProductDetails))]
    [InlineData(nameof(ProductsController.DeactivateProduct))]
    [InlineData(nameof(ProductsController.ReactivateProduct))]
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
    [InlineData(nameof(OrdersController.ListOrders))]
    [InlineData(nameof(OrdersController.CreateOrder))]
    [InlineData(nameof(OrdersController.GetOrderById))]
    public void OrdersEndpoints_ShouldRequireAuthorization(string actionName)
    {
        var method = GetAction(typeof(OrdersController), actionName);

        Assert.True(HasAuthorizeAttribute(method), $"{actionName} should require authorization.");
    }

    [Fact]
    public void AssistantQueryEndpoint_ShouldRequireAuthorization()
    {
        var method = GetAction(typeof(AssistantController), nameof(AssistantController.Query));

        Assert.True(HasAuthorizeAttribute(method), "Assistant query should require authorization.");
    }

    [Theory]
    [InlineData(nameof(ProductsController.CreateProduct), typeof(HttpPostAttribute))]
    [InlineData(nameof(ProductsController.UpdateProductDetails), typeof(HttpPutAttribute))]
    [InlineData(nameof(ProductsController.DeactivateProduct), typeof(HttpDeleteAttribute))]
    [InlineData(nameof(ProductsController.ReactivateProduct), typeof(HttpPostAttribute))]
    public void CatalogProtectedEndpoints_ShouldUseExpectedHttpMethods(string actionName, Type attributeType)
    {
        var method = GetAction(typeof(ProductsController), actionName);

        Assert.Contains(method.GetCustomAttributes(), attribute => attribute.GetType() == attributeType);
    }

    [Theory]
    [InlineData(nameof(OrdersController.ListOrders), typeof(HttpGetAttribute))]
    [InlineData(nameof(OrdersController.CreateOrder), typeof(HttpPostAttribute))]
    [InlineData(nameof(OrdersController.GetOrderById), typeof(HttpGetAttribute))]
    public void OrdersEndpoints_ShouldUseExpectedHttpMethods(string actionName, Type attributeType)
    {
        var method = GetAction(typeof(OrdersController), actionName);

        Assert.Contains(method.GetCustomAttributes(), attribute => attribute.GetType() == attributeType);
    }

    [Fact]
    public void AssistantQueryEndpoint_ShouldUsePost()
    {
        var method = GetAction(typeof(AssistantController), nameof(AssistantController.Query));

        Assert.Contains(method.GetCustomAttributes(), attribute => attribute.GetType() == typeof(HttpPostAttribute));
    }

    [Fact]
    public void ListOrdersResponse_ShouldExposeOrderSummariesOnly()
    {
        var listProperties = typeof(ListOrdersResponse)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();
        var summaryProperties = typeof(OrderSummaryResponse)
            .GetProperties()
            .Select(property => property.Name)
            .OrderBy(name => name)
            .ToArray();

        Assert.DoesNotContain("Lines", listProperties);
        Assert.DoesNotContain("Lines", summaryProperties);
        Assert.Equal(
            new[]
            {
                nameof(OrderSummaryResponse.CreatedAt),
                nameof(OrderSummaryResponse.LineCount),
                nameof(OrderSummaryResponse.OrderId),
                nameof(OrderSummaryResponse.Status),
                nameof(OrderSummaryResponse.TotalAmount)
            },
            summaryProperties);
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

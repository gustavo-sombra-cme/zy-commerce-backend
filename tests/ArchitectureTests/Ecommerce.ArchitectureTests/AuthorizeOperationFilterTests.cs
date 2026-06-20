using System.Reflection;
using Ecommerce.Api.Controllers.Catalog;
using Ecommerce.Api.OpenApi;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecommerce.ArchitectureTests;

public sealed class AuthorizeOperationFilterTests
{
    [Fact]
    public void Apply_WithAuthorizedAction_AddsAuthorizationHeaderParameter()
    {
        var operation = new OpenApiOperation();
        var context = CreateContext(nameof(ProductsController.CreateProduct));
        var filter = new AuthorizationHeaderOperationFilter();

        filter.Apply(operation, context);

        Assert.NotNull(operation.Parameters);
        var parameter = Assert.Single(operation.Parameters);
        Assert.Equal("Authorization", parameter.Name);
        Assert.Equal(ParameterLocation.Header, parameter.In);
        Assert.True(parameter.Required);
    }

    [Fact]
    public void Apply_WithPublicAction_DoesNotAddAuthorizationHeaderParameter()
    {
        var operation = new OpenApiOperation();
        var context = CreateContext(nameof(ProductsController.SearchProducts));
        var filter = new AuthorizationHeaderOperationFilter();

        filter.Apply(operation, context);

        Assert.Null(operation.Parameters);
    }

    private static OperationFilterContext CreateContext(string actionName)
    {
        var method = typeof(ProductsController).GetMethod(actionName)
            ?? throw new InvalidOperationException($"Could not find {nameof(ProductsController)}.{actionName}.");

        return new OperationFilterContext(
            new ApiDescription
            {
                ActionDescriptor = new ActionDescriptor
                {
                    EndpointMetadata = []
                }
            },
            null!,
            new SchemaRepository(),
            new OpenApiDocument(),
            method);
    }
}

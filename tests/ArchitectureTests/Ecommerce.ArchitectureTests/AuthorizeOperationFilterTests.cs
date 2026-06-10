using System.Reflection;
using Ecommerce.Api.Controllers.Catalog;
using Ecommerce.Api.OpenApi;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecommerce.ArchitectureTests;

public sealed class AuthorizeOperationFilterTests
{
    [Fact]
    public void Apply_WithAuthorizedAction_AddsBearerSecurityRequirement()
    {
        var operation = new OpenApiOperation();
        var context = CreateContext(nameof(ProductsController.CreateProduct));
        var filter = new AuthorizeOperationFilter();

        filter.Apply(operation, context);

        Assert.NotNull(operation.Security);
        var requirement = Assert.Single(operation.Security);
        var scheme = Assert.Single(requirement.Keys);
        Assert.IsType<OpenApiSecuritySchemeReference>(scheme);
        Assert.Equal("Bearer", scheme.Reference.Id);
    }

    [Fact]
    public void Apply_WithPublicAction_DoesNotAddSecurityRequirement()
    {
        var operation = new OpenApiOperation();
        var context = CreateContext(nameof(ProductsController.SearchProducts));
        var filter = new AuthorizeOperationFilter();

        filter.Apply(operation, context);

        Assert.Null(operation.Security);
    }

    private static OperationFilterContext CreateContext(string actionName)
    {
        var method = typeof(ProductsController).GetMethod(actionName)
            ?? throw new InvalidOperationException($"Could not find {nameof(ProductsController)}.{actionName}.");

        return new OperationFilterContext(
            new ApiDescription(),
            null!,
            new SchemaRepository(),
            new OpenApiDocument(),
            method);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecommerce.Api.OpenApi;

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    private const string BearerSecuritySchemeId = "Bearer";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requiresAuthorization = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (!requiresAuthorization)
        {
            return;
        }

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        var document = new OpenApiDocument();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(BearerSecuritySchemeId, document, BearerSecuritySchemeId)] = new List<string>()
        });
    }
}

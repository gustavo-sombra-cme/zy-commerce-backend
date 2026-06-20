using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecommerce.Api.OpenApi;

public sealed class AuthorizationHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (AllowsAnonymous(context) || !RequiresAuthorization(context))
        {
            return;
        }

        operation.Parameters ??= new List<IOpenApiParameter>();

        if (operation.Parameters.Any(parameter =>
                parameter.In == ParameterLocation.Header &&
                string.Equals(parameter.Name, "Authorization", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Required = true,
            Description = "Enter: Bearer {accessToken}",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            }
        });
    }

    private static bool RequiresAuthorization(OperationFilterContext context)
    {
        return context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<IAuthorizeData>()
                .Any()
            || context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<IAuthorizeData>()
                .Any() == true
            || context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<IAuthorizeData>()
                .Any();
    }

    private static bool AllowsAnonymous(OperationFilterContext context)
    {
        return context.ApiDescription.ActionDescriptor.EndpointMetadata
                .OfType<IAllowAnonymous>()
                .Any()
            || context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<IAllowAnonymous>()
                .Any() == true
            || context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<IAllowAnonymous>()
                .Any();
    }
}
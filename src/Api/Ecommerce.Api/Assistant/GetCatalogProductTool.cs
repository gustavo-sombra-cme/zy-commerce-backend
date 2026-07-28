using System.Text.Json;
using Ecommerce.Catalog.Application.Products.GetProductById;
using MediatR;

namespace Ecommerce.Api.Assistant;

public sealed class GetCatalogProductTool(ISender sender) : ICatalogAgentTool
{
    public const string ToolName = "catalog_get_product";
    private static readonly HashSet<string> AllowedProperties = ["productId"];

    public string Name => ToolName;

    public AssistantToolDefinition Definition { get; } = new(
        ToolName,
        "Get full active public catalog details for a product returned by a successful search in this execution.",
        """
        {"type":"object","additionalProperties":false,"required":["productId"],"properties":{"productId":{"type":"string","format":"uuid"}}}
        """);

    public async Task<AssistantToolExecutionResult> ExecuteAsync(
        string argumentsJson,
        CatalogAgentToolExecutionContext executionContext,
        CancellationToken cancellationToken)
    {
        if (!executionContext.ExecutionContext.AllowedDataScopes.Contains(AssistantDataScopes.CatalogPublic, StringComparer.Ordinal))
        {
            return AssistantToolExecutionResult.Failure(Name, "scope_not_allowed", "Public catalog scope is not allowed.");
        }

        if (string.IsNullOrWhiteSpace(argumentsJson))
        {
            return AssistantToolExecutionResult.Failure(Name, "missing_arguments", "Tool arguments are required.");
        }

        string? rawProductId;
        try
        {
            using var document = JsonDocument.Parse(argumentsJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object
                || document.RootElement.EnumerateObject().Any(property => !AllowedProperties.Contains(property.Name))
                || !document.RootElement.TryGetProperty("productId", out var productIdElement)
                || productIdElement.ValueKind != JsonValueKind.String)
            {
                return AssistantToolExecutionResult.Failure(Name, "invalid_arguments", "A productId is required and no additional properties are allowed.");
            }

            rawProductId = productIdElement.GetString();
        }
        catch (JsonException)
        {
            return AssistantToolExecutionResult.Failure(Name, "invalid_arguments", "Tool arguments are not valid JSON.");
        }

        if (!Guid.TryParse(rawProductId, out var productId))
        {
            return AssistantToolExecutionResult.Failure(Name, "invalid_product_id", "The product identifier is invalid.");
        }

        if (!executionContext.TrustedProductIds.Contains(productId))
        {
            return AssistantToolExecutionResult.Failure(Name, "untrusted_product_id", "The product identifier was not returned by a trusted search in this execution.");
        }

        try
        {
            var product = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);
            if (product is null || !product.IsActive)
            {
                return AssistantToolExecutionResult.Failure(Name, "product_not_found", "The active product was not found.");
            }

            return AssistantToolExecutionResult.Success(Name, new CatalogProductToolResult(AssistantProductMapper.ToCard(product)));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return AssistantToolExecutionResult.Failure(Name, "query_failed", "Product lookup failed safely.");
        }
    }
}

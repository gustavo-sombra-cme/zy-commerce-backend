using System.Text.Json;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class SearchCatalogProductsTool(
    ISender sender,
    IOptions<CatalogAgentOptions> options) : ICatalogAgentTool
{
    public const string ToolName = "catalog_search_products";
    private const int MaximumSearchTextLength = 200;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly HashSet<string> AllowedProperties =
        ["searchText", "maximumPrice", "pageNumber", "pageSize"];

    public string Name => ToolName;

    public AssistantToolDefinition Definition { get; } = new(
        ToolName,
        "Search active public catalog products by name, SKU, description, and optional strict maximum price.",
        """
        {"type":"object","additionalProperties":false,"properties":{"searchText":{"type":["string","null"],"maxLength":200},"maximumPrice":{"type":["number","null"],"minimum":0},"pageNumber":{"type":["integer","null"],"minimum":1},"pageSize":{"type":["integer","null"],"minimum":1,"maximum":20}}}
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

        SearchArguments? arguments;
        try
        {
            using var document = JsonDocument.Parse(argumentsJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object
                || document.RootElement.EnumerateObject().Any(property => !AllowedProperties.Contains(property.Name)))
            {
                return AssistantToolExecutionResult.Failure(Name, "invalid_arguments", "Tool arguments contain unsupported properties.");
            }

            arguments = JsonSerializer.Deserialize<SearchArguments>(argumentsJson, JsonOptions);
        }
        catch (JsonException)
        {
            return AssistantToolExecutionResult.Failure(Name, "invalid_arguments", "Tool arguments are not valid JSON.");
        }

        if (arguments is null)
        {
            return AssistantToolExecutionResult.Failure(Name, "invalid_arguments", "Tool arguments are invalid.");
        }

        var searchText = string.IsNullOrWhiteSpace(arguments.SearchText) ? null : arguments.SearchText.Trim();
        var pageNumber = arguments.PageNumber ?? 1;
        var pageSize = arguments.PageSize ?? 10;
        if ((searchText?.Length ?? 0) > MaximumSearchTextLength
            || arguments.MaximumPrice < 0
            || pageNumber < 1
            || pageSize < 1
            || pageSize > options.Value.EffectiveMaximumSearchPageSize)
        {
            return AssistantToolExecutionResult.Failure(Name, "invalid_arguments", "Tool arguments are outside allowed limits.");
        }

        try
        {
            var result = await sender.Send(
                new SearchProductsQuery(searchText, true, pageNumber, pageSize, arguments.MaximumPrice),
                cancellationToken);
            var products = result.Items
                .Where(product => product.IsActive)
                .Select(AssistantProductMapper.ToCard)
                .ToArray();

            return AssistantToolExecutionResult.Success(
                Name,
                new CatalogSearchToolResult(products, result.TotalCount, result.PageNumber, result.PageSize, arguments.MaximumPrice));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return AssistantToolExecutionResult.Failure(Name, "query_failed", "Catalog search failed safely.");
        }
    }

    private sealed record SearchArguments(
        string? SearchText,
        decimal? MaximumPrice,
        int? PageNumber,
        int? PageSize);
}

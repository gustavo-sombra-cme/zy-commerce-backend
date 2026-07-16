namespace Ecommerce.Api.Assistant;

public sealed record CatalogAgentToolExecutionContext(
    AssistantExecutionContext ExecutionContext,
    IReadOnlySet<Guid> TrustedProductIds);

public sealed record AssistantToolExecutionResult(
    bool Succeeded,
    string ToolName,
    object? Data,
    string? ErrorCode,
    string? ErrorMessage)
{
    public static AssistantToolExecutionResult Success(string toolName, object data) =>
        new(true, toolName, data, null, null);

    public static AssistantToolExecutionResult Failure(string toolName, string errorCode, string errorMessage) =>
        new(false, toolName, null, errorCode, errorMessage);
}

public sealed record CatalogSearchToolResult(
    IReadOnlyCollection<AssistantProductCardDto> Products,
    int TotalCount,
    int PageNumber,
    int PageSize,
    decimal? MaximumPrice);

public sealed record CatalogProductToolResult(AssistantProductCardDto Product);

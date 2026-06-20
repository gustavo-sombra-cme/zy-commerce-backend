namespace Ecommerce.Api.Assistant;

public sealed record AssistantQueryResponse(
    string Answer,
    IReadOnlyCollection<string> ToolsUsed,
    string DataScope,
    bool Unsupported,
    string? ResponseType = null,
    object? Data = null);

namespace Ecommerce.Api.Assistant;

public enum AssistantMessageRole
{
    User,
    Assistant,
    Tool
}

public enum AssistantModelFinishReason
{
    Completed,
    ToolCallsRequested,
    MaximumTokens,
    Refused,
    Failed
}

public enum AssistantResponseFormat
{
    CatalogAgent
}

public enum CatalogAgentFinalResponseType
{
    Text,
    CatalogProduct,
    CatalogProducts
}

public sealed record AssistantConversationMessage(
    AssistantMessageRole Role,
    string Content,
    string? ToolCallId = null,
    string? ToolName = null);

public sealed record AssistantToolDefinition(
    string Name,
    string Description,
    string InputJsonSchema);

public sealed record AssistantToolCall(
    string Id,
    string Name,
    string ArgumentsJson);

public sealed record CatalogAgentFinalAnswer(
    string Message,
    CatalogAgentFinalResponseType ResponseType,
    IReadOnlyCollection<Guid> SelectedProductIds,
    decimal? MaximumPrice,
    bool NeedsClarification);

public sealed record AssistantModelRequest(
    string SystemInstructions,
    IReadOnlyCollection<AssistantConversationMessage> Messages,
    IReadOnlyCollection<AssistantToolDefinition> Tools,
    AssistantResponseFormat ResponseFormat);

public sealed record AssistantModelResponse(
    string? Text,
    IReadOnlyCollection<AssistantToolCall> ToolCalls,
    AssistantModelFinishReason FinishReason,
    CatalogAgentFinalAnswer? FinalAnswer)
{
    public static AssistantModelResponse Failed() =>
        new(null, Array.Empty<AssistantToolCall>(), AssistantModelFinishReason.Failed, null);
}

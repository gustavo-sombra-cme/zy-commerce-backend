namespace Ecommerce.Api.Assistant;

public sealed record CatalogAgentRequest(
    string UserMessage,
    IReadOnlyCollection<AssistantConversationMessage> Conversation,
    AssistantExecutionContext ExecutionContext,
    AssistantIntent? CompatibilityIntent = null);

public sealed record AssistantExecutionContext(
    string CorrelationId,
    Guid? AuthenticatedUserId,
    IReadOnlyCollection<string> AllowedDataScopes);

namespace Ecommerce.Api.Assistant;

public interface ICatalogAgentTool
{
    string Name { get; }
    AssistantToolDefinition Definition { get; }

    Task<AssistantToolExecutionResult> ExecuteAsync(
        string argumentsJson,
        CatalogAgentToolExecutionContext executionContext,
        CancellationToken cancellationToken);
}

public interface ICatalogAgentToolRegistry
{
    IReadOnlyCollection<AssistantToolDefinition> Definitions { get; }
    bool TryGetTool(string toolName, out ICatalogAgentTool? tool);
}

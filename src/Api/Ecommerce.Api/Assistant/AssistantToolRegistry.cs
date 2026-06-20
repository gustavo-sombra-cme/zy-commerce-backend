namespace Ecommerce.Api.Assistant;

public sealed class AssistantToolRegistry
{
    private static readonly string[] Tools =
    [
        AssistantToolNames.CatalogSearch,
        AssistantToolNames.CatalogGetProduct,
        AssistantToolNames.OrdersSearch,
        AssistantToolNames.OrdersGetOrder,
        AssistantToolNames.OrdersAnalyze
    ];

    public IReadOnlyCollection<string> GetAllowedTools() => Tools;

    public bool IsAllowed(string toolName) =>
        Tools.Contains(toolName, StringComparer.Ordinal);
}

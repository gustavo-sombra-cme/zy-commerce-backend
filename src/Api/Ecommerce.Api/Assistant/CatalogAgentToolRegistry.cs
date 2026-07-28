namespace Ecommerce.Api.Assistant;

public sealed class CatalogAgentToolRegistry : ICatalogAgentToolRegistry
{
    private readonly IReadOnlyDictionary<string, ICatalogAgentTool> _tools;

    public CatalogAgentToolRegistry(IEnumerable<ICatalogAgentTool> tools)
    {
        var registered = new Dictionary<string, ICatalogAgentTool>(StringComparer.Ordinal);
        foreach (var tool in tools)
        {
            if (!registered.TryAdd(tool.Name, tool))
            {
                throw new InvalidOperationException($"Duplicate catalog agent tool registration: {tool.Name}.");
            }
        }

        _tools = registered;
        Definitions = registered.Values.Select(tool => tool.Definition).ToArray();
    }

    public IReadOnlyCollection<AssistantToolDefinition> Definitions { get; }

    public bool TryGetTool(string toolName, out ICatalogAgentTool? tool) =>
        _tools.TryGetValue(toolName, out tool);
}

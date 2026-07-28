namespace Ecommerce.Api.Assistant;

public sealed class CatalogAgentOptions
{
    public const string SectionName = "Assistant:CatalogAgent";
    public const int HardMaximumIterations = 10;
    public const int HardMaximumToolCallsPerIteration = 5;
    public const int HardMaximumConversationMessages = 20;
    public const int HardMaximumSearchPageSize = 20;

    public bool Enabled { get; init; } = true;
    public int MaximumIterations { get; init; } = 6;
    public int MaximumToolCallsPerIteration { get; init; } = 3;
    public int MaximumConversationMessages { get; init; } = 10;
    public int MaximumSearchPageSize { get; init; } = 20;

    public int EffectiveMaximumIterations => Math.Clamp(MaximumIterations, 1, HardMaximumIterations);
    public int EffectiveMaximumToolCallsPerIteration => Math.Clamp(MaximumToolCallsPerIteration, 1, HardMaximumToolCallsPerIteration);
    public int EffectiveMaximumConversationMessages => Math.Clamp(MaximumConversationMessages, 1, HardMaximumConversationMessages);
    public int EffectiveMaximumSearchPageSize => Math.Clamp(MaximumSearchPageSize, 1, HardMaximumSearchPageSize);
}

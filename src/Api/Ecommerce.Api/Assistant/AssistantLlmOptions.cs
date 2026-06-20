namespace Ecommerce.Api.Assistant;

public sealed class AssistantLlmOptions
{
    public const string SectionName = "Assistant:Llm";

    public bool Enabled { get; init; }

    public string Endpoint { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public string ApiKeyEnvironmentVariable { get; init; } = "ECOMMERCE_ASSISTANT_LLM_API_KEY";

    public string ApiKey { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 10;

    public int MaxResponseCharacters { get; init; } = 4000;

    public TimeSpan Timeout =>
        TimeSpan.FromSeconds(Math.Clamp(TimeoutSeconds, 1, 30));
}

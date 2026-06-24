namespace Ecommerce.Api.Assistant;

public sealed class AssistantLlmOptions
{
    public const string SectionName = "Assistant:Llm";
    public const string GeminiProvider = "Gemini";
    public const string OpenAiProvider = "OpenAI";

    public bool Enabled { get; init; }

    public string Provider { get; init; } = OpenAiProvider;

    public string Endpoint { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public string ApiKeyEnvironmentVariable { get; init; } = "ECOMMERCE_ASSISTANT_LLM_API_KEY";

    public string ApiKey { get; init; } = string.Empty;

    public string GeminiEndpoint { get; init; } = "https://generativelanguage.googleapis.com/v1beta";

    public string GeminiModel { get; init; } = "gemini-2.5-flash";

    public string GeminiApiKeyEnvironmentVariable { get; init; } = "ECOMMERCE_ASSISTANT_GEMINI_API_KEY";

    public string GeminiApiKey { get; init; } = string.Empty;

    public int TimeoutSeconds { get; init; } = 10;

    public int MaxResponseCharacters { get; init; } = 4000;

    public TimeSpan Timeout =>
        TimeSpan.FromSeconds(Math.Clamp(TimeoutSeconds, 1, 30));

    public string ResolvedProvider =>
        Environment.GetEnvironmentVariable("ECOMMERCE_ASSISTANT_LLM_PROVIDER")
            ?? Provider;

    public bool IsGeminiProvider =>
        string.Equals(ResolvedProvider, GeminiProvider, StringComparison.OrdinalIgnoreCase);

    public string ResolvedEndpoint =>
        IsGeminiProvider
            ? Environment.GetEnvironmentVariable("ECOMMERCE_ASSISTANT_GEMINI_ENDPOINT") ?? GeminiEndpoint
            : Endpoint;

    public string ResolvedModel =>
        IsGeminiProvider
            ? Environment.GetEnvironmentVariable("ECOMMERCE_ASSISTANT_GEMINI_MODEL") ?? GeminiModel
            : Model;

    public string ResolvedApiKeyEnvironmentVariable =>
        IsGeminiProvider
            ? GeminiApiKeyEnvironmentVariable
            : ApiKeyEnvironmentVariable;

    public bool TryResolveApiKey(out string apiKey)
    {
        apiKey = string.Empty;

        if (IsGeminiProvider)
        {
            apiKey = Environment.GetEnvironmentVariable("ECOMMERCE_ASSISTANT_GEMINI_API_KEY") ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(apiKey)
            && !string.IsNullOrWhiteSpace(ResolvedApiKeyEnvironmentVariable))
        {
            apiKey = Environment.GetEnvironmentVariable(ResolvedApiKeyEnvironmentVariable) ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = IsGeminiProvider ? GeminiApiKey : ApiKey;
        }

        apiKey = apiKey.Trim();
        return !string.IsNullOrWhiteSpace(apiKey);
    }
}

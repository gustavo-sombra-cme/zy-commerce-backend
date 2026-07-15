using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class GeminiAssistantLlmClient(
    HttpClient httpClient,
    IOptions<AssistantLlmOptions> options,
    ILogger<GeminiAssistantLlmClient> logger) : IAssistantLlmClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<string?> CreateIntentPlanJsonAsync(
        string question,
        CancellationToken cancellationToken)
    {
        var llmOptions = options.Value;

        if (!llmOptions.Enabled
            || !llmOptions.IsGeminiProvider
            || string.IsNullOrWhiteSpace(question)
            || string.IsNullOrWhiteSpace(llmOptions.ResolvedModel)
            || !TryGetEndpoint(llmOptions, out var endpoint)
            || !llmOptions.TryResolveApiKey(out var apiKey))
        {
            return null;
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            CreateGenerateContentUri(endpoint, llmOptions.ResolvedModel, apiKey));
        request.Content = new StringContent(
            JsonSerializer.Serialize(CreateRequestBody(question), JsonOptions),
            Encoding.UTF8,
            "application/json");

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await LogProviderErrorAsync(response, cancellationToken);
            return null;
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        return TryExtractCandidateText(responseBody);
    }

    private static object CreateRequestBody(string question) =>
        new
        {
            systemInstruction = new
            {
                parts = new[]
                {
                    new { text = SystemPrompt }
                }
            },
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = question }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0,
                responseMimeType = "application/json"
            }
        };

    private static string? TryExtractCandidateText(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);

            if (!document.RootElement.TryGetProperty("candidates", out var candidates)
                || candidates.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            foreach (var candidate in candidates.EnumerateArray())
            {
                if (!candidate.TryGetProperty("content", out var content)
                    || content.ValueKind != JsonValueKind.Object
                    || !content.TryGetProperty("parts", out var parts)
                    || parts.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var part in parts.EnumerateArray())
                {
                    if (part.TryGetProperty("text", out var text)
                        && text.ValueKind == JsonValueKind.String
                        && !string.IsNullOrWhiteSpace(text.GetString()))
                    {
                        return text.GetString();
                    }
                }
            }

            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private async Task LogProviderErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var error = await TryReadProviderErrorAsync(response, cancellationToken);

        logger.LogWarning(
            "Assistant Gemini provider error diagnostics: statusCode={StatusCode}, geminiErrorCode={GeminiErrorCode}, geminiErrorStatus={GeminiErrorStatus}, sanitizedErrorMessage={SanitizedErrorMessage}.",
            (int)response.StatusCode,
            error.Code,
            error.Status,
            error.Message);
    }

    private static async Task<ProviderError> TryReadProviderErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(body))
        {
            return ProviderError.Empty;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            var error = document.RootElement.TryGetProperty("error", out var errorElement)
                && errorElement.ValueKind == JsonValueKind.Object
                ? errorElement
                : document.RootElement;

            return new ProviderError(
                TryReadCode(error),
                TryReadSafeString(error, "status"),
                SanitizeErrorMessage(TryReadString(error, "message")));
        }
        catch (JsonException)
        {
            return ProviderError.Empty;
        }
    }

    private static string? TryReadCode(JsonElement element)
    {
        if (!element.TryGetProperty("code", out var value))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.String => SanitizeMetadataValue(value.GetString()),
            _ => null
        };
    }

    private static string? TryReadSafeString(JsonElement element, string propertyName) =>
        SanitizeMetadataValue(TryReadString(element, propertyName));

    private static string? TryReadString(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var value)
            || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return value.GetString();
    }

    private static string? SanitizeMetadataValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= 80
            && trimmed.All(character => char.IsLetterOrDigit(character)
                || character is '_' or '-' or '.')
            ? trimmed
            : null;
    }

    private static string? SanitizeErrorMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        var sanitized = message.Trim();
        sanitized = System.Text.RegularExpressions.Regex.Replace(
            sanitized,
            @"'[^']*'|""[^""]*""",
            "[redacted]",
            System.Text.RegularExpressions.RegexOptions.None,
            TimeSpan.FromMilliseconds(100));
        sanitized = System.Text.RegularExpressions.Regex.Replace(
            sanitized,
            @"\b(AIza[A-Za-z0-9_-]+|Bearer\s+[A-Za-z0-9._-]+|[A-Za-z0-9_-]{32,})\b",
            "[redacted]",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase,
            TimeSpan.FromMilliseconds(100));

        if (sanitized.Contains("authorization", StringComparison.OrdinalIgnoreCase)
            || sanitized.Contains("api key", StringComparison.OrdinalIgnoreCase)
            || sanitized.Contains("token", StringComparison.OrdinalIgnoreCase)
            || sanitized.Contains("secret", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return sanitized.Length > 300
            ? sanitized[..300]
            : sanitized;
    }

    private static bool TryGetEndpoint(
        AssistantLlmOptions options,
        out Uri endpoint)
    {
        return Uri.TryCreate(options.ResolvedEndpoint, UriKind.Absolute, out endpoint!)
            && endpoint.Scheme == Uri.UriSchemeHttps;
    }

    private static Uri CreateGenerateContentUri(
        Uri endpoint,
        string model,
        string apiKey)
    {
        var builder = new UriBuilder(endpoint);
        var basePath = builder.Path.TrimEnd('/');
        builder.Path = $"{basePath}/models/{Uri.EscapeDataString(model)}:generateContent";
        builder.Query = $"key={Uri.EscapeDataString(apiKey)}";
        return builder.Uri;
    }

    private const string SystemPrompt = """
        You are an intent interpreter for a read-only ecommerce backend assistant.
        Return only one JSON object with this shape:
        {"kind":"RecentOrders","tools":["orders_search"],"arguments":{}}
        Allowed kind values: Unsupported, RecentOrders, TotalSpend, ProductsOrdered, OrdersContainingProduct, OrdersAboveAmount, OrdersContainingProductsOverAmount, ProductFrequency, CatalogSearchProducts, CatalogProductsUnderPrice, CatalogGetProduct.
        Allowed tool values: catalog_search, catalog_get_product, orders_search, orders_get_order, orders_analyze.
        Use only the tools required for the selected kind.
        Arguments may include only amount, searchText, or productId when required by the selected kind.
        Use CatalogSearchProducts with catalog_search and searchText for read-only product discovery questions such as "show me Galaxy products", "search for SKU ABC123", or "do you have headphones".
        Never include userId, buyerId, ownerId, customerId, subject, authorization, token, password, SQL, connection strings, internal prompts, or secrets.
        Return Unsupported with empty tools and empty arguments for mutating, admin, SQL, database, token, cross-user, internal, unsafe, or unclear requests.
        Do not answer the user and do not describe the plan.
        """;

    private sealed record ProviderError(
        string? Code,
        string? Status,
        string? Message)
    {
        public static ProviderError Empty { get; } = new(null, null, null);
    }
}

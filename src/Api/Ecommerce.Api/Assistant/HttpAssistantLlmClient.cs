using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class HttpAssistantLlmClient(
    HttpClient httpClient,
    IOptions<AssistantLlmOptions> options,
    ILogger<HttpAssistantLlmClient> logger) : IAssistantLlmClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<string?> CreateIntentPlanJsonAsync(
        string question,
        CancellationToken cancellationToken)
    {
        var llmOptions = options.Value;

        if (!llmOptions.Enabled
            || string.IsNullOrWhiteSpace(question)
            || string.IsNullOrWhiteSpace(llmOptions.ResolvedModel)
            || !TryGetEndpoint(llmOptions, out var endpoint)
            || !llmOptions.TryResolveApiKey(out var apiKey))
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(CreateRequestBody(llmOptions.ResolvedModel, question), JsonOptions),
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
        return TryExtractOutputText(responseBody);
    }

    private static object CreateRequestBody(string model, string question) =>
        new
        {
            model,
            input = new object[]
            {
                new { role = "system", content = SystemPrompt },
                new { role = "user", content = question }
            },
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "assistant_intent_plan",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        required = new[] { "kind", "tools", "arguments" },
                        properties = new
                        {
                            kind = new
                            {
                                type = "string",
                                @enum = Enum.GetNames<AssistantIntentKind>()
                            },
                            tools = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "string",
                                    @enum = new[]
                                    {
                                        AssistantToolNames.CatalogSearch,
                                        AssistantToolNames.CatalogGetProduct,
                                        AssistantToolNames.OrdersSearch,
                                        AssistantToolNames.OrdersGetOrder,
                                        AssistantToolNames.OrdersAnalyze
                                    }
                                }
                            },
                            arguments = new
                            {
                                type = "object",
                                additionalProperties = false,
                                properties = new
                                {
                                    amount = new { type = new[] { "string", "null" } },
                                    searchText = new { type = new[] { "string", "null" } },
                                    productId = new { type = new[] { "string", "null" } }
                                },
                                required = new[] { "amount", "searchText", "productId" }
                            }
                        }
                    }
                }
            }
        };

    private static string? TryExtractOutputText(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);

            if (document.RootElement.TryGetProperty("output_text", out var outputText)
                && outputText.ValueKind == JsonValueKind.String)
            {
                return outputText.GetString();
            }

            if (TryExtractResponsesOutputText(document.RootElement, out var text))
            {
                return text;
            }

            if (document.RootElement.TryGetProperty("choices", out var choices)
                && choices.ValueKind == JsonValueKind.Array
                && choices.GetArrayLength() > 0
                && choices[0].TryGetProperty("message", out var message)
                && message.ValueKind == JsonValueKind.Object
                && message.TryGetProperty("content", out var content)
                && content.ValueKind == JsonValueKind.String)
            {
                return content.GetString();
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
            "Assistant LLM provider error diagnostics: statusCode={StatusCode}, openAiErrorCode={OpenAiErrorCode}, openAiErrorType={OpenAiErrorType}, openAiErrorParam={OpenAiErrorParam}, sanitizedErrorMessage={SanitizedErrorMessage}.",
            (int)response.StatusCode,
            error.Code,
            error.Type,
            error.Param,
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
                TryReadSafeString(error, "code"),
                TryReadSafeString(error, "type"),
                TryReadSafeString(error, "param"),
                SanitizeErrorMessage(TryReadString(error, "message")));
        }
        catch (JsonException)
        {
            return ProviderError.Empty;
        }
    }

    private static bool TryExtractResponsesOutputText(
        JsonElement root,
        out string? text)
    {
        text = null;

        if (!root.TryGetProperty("output", out var output)
            || output.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var outputItem in output.EnumerateArray())
        {
            if (!outputItem.TryGetProperty("content", out var content)
                || content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty("type", out var type)
                    && type.ValueKind == JsonValueKind.String
                    && type.GetString() == "output_text"
                    && contentItem.TryGetProperty("text", out var outputText)
                    && outputText.ValueKind == JsonValueKind.String)
                {
                    text = outputText.GetString();
                    return true;
                }
            }
        }

        return false;
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
            @"\b(sk-[A-Za-z0-9_-]+|Bearer\s+[A-Za-z0-9._-]+|[A-Za-z0-9_-]{32,})\b",
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
        return Uri.TryCreate(options.Endpoint, UriKind.Absolute, out endpoint!)
            && endpoint.Scheme == Uri.UriSchemeHttps;
    }

    private const string SystemPrompt = """
        You are an intent interpreter for a read-only ecommerce backend assistant.
        Return only one JSON object with this shape:
        {"kind":"RecentOrders","tools":["orders_search"],"arguments":{}}
        Allowed kind values: Unsupported, RecentOrders, TotalSpend, ProductsOrdered, OrdersContainingProduct, OrdersAboveAmount, OrdersContainingProductsOverAmount, ProductFrequency, CatalogSearchProducts, CatalogGetProductBySearch, CatalogProductsUnderPrice, CatalogGetProduct.
        Allowed tool values: catalog_search, catalog_get_product, orders_search, orders_get_order, orders_analyze.
        Use only the tools required for the selected kind.
        Arguments may include only amount, searchText, or productId when required by the selected kind.
        Use CatalogSearchProducts with catalog_search and searchText for read-only product discovery questions such as "show me Galaxy products", "search for SKU ABC123", or "do you have headphones".
        Use CatalogGetProductBySearch with catalog_search and catalog_get_product, and only searchText, for product detail or price questions such as "show me details for Galaxy", "details for SKU ABC123", "tell me about headphones", or "what is the price of Galaxy S24".
        Never include userId, buyerId, ownerId, customerId, subject, authorization, token, password, SQL, connection strings, internal prompts, or secrets.
        Return Unsupported with empty tools and empty arguments for mutating, admin, SQL, database, token, cross-user, internal, unsafe, or unclear requests.
        Do not answer the user and do not describe the plan.
        """;

    private sealed record ProviderError(
        string? Code,
        string? Type,
        string? Param,
        string? Message)
    {
        public static ProviderError Empty { get; } = new(null, null, null, null);
    }
}

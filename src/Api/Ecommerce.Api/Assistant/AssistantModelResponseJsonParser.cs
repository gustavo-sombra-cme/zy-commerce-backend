using System.Text.Json;

namespace Ecommerce.Api.Assistant;

public sealed class AssistantModelResponseJsonParser
{
    private const int MaximumResponseCharacters = 16_000;
    private const int MaximumTextCharacters = 4_000;
    private const int MaximumArgumentsCharacters = 4_000;
    private static readonly HashSet<string> AllowedRootProperties =
    [
        "finishReason",
        "text",
        "toolCalls",
        "responseType",
        "selectedProductIds",
        "maximumPrice",
        "needsClarification"
    ];
    private static readonly HashSet<string> AllowedToolCallProperties = ["id", "name", "argumentsJson"];

    public AssistantModelResponse? Parse(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Length > MaximumResponseCharacters)
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object
                || root.EnumerateObject().Any(property => !AllowedRootProperties.Contains(property.Name))
                || !TryReadEnum(root, "finishReason", out AssistantModelFinishReason finishReason)
                || !TryReadNullableString(root, "text", MaximumTextCharacters, out var text)
                || !TryReadToolCalls(root, out var toolCalls)
                || !TryReadNullableEnum(root, "responseType", out CatalogAgentFinalResponseType? responseType)
                || !TryReadProductIds(root, out var selectedProductIds)
                || !TryReadNullableDecimal(root, "maximumPrice", out var maximumPrice)
                || !TryReadBoolean(root, "needsClarification", out var needsClarification))
            {
                return null;
            }

            CatalogAgentFinalAnswer? finalAnswer = null;
            if (finishReason == AssistantModelFinishReason.Completed)
            {
                if (string.IsNullOrWhiteSpace(text) || responseType is null)
                {
                    return null;
                }

                finalAnswer = new CatalogAgentFinalAnswer(
                    text.Trim(),
                    responseType.Value,
                    selectedProductIds,
                    maximumPrice,
                    needsClarification);
            }

            return new AssistantModelResponse(text, toolCalls, finishReason, finalAnswer);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool TryReadToolCalls(JsonElement root, out IReadOnlyCollection<AssistantToolCall> toolCalls)
    {
        toolCalls = Array.Empty<AssistantToolCall>();
        if (!root.TryGetProperty("toolCalls", out var element) || element.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        var parsed = new List<AssistantToolCall>();
        foreach (var item in element.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object
                || item.EnumerateObject().Any(property => !AllowedToolCallProperties.Contains(property.Name))
                || !TryReadRequiredString(item, "id", 100, out var id)
                || !TryReadRequiredString(item, "name", 100, out var name)
                || !TryReadRequiredString(item, "argumentsJson", MaximumArgumentsCharacters, out var argumentsJson))
            {
                return false;
            }

            parsed.Add(new AssistantToolCall(id, name, argumentsJson));
        }

        toolCalls = parsed;
        return true;
    }

    private static bool TryReadProductIds(JsonElement root, out IReadOnlyCollection<Guid> productIds)
    {
        productIds = Array.Empty<Guid>();
        if (!root.TryGetProperty("selectedProductIds", out var element) || element.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        var parsed = new List<Guid>();
        foreach (var item in element.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String || !Guid.TryParse(item.GetString(), out var productId))
            {
                return false;
            }

            parsed.Add(productId);
        }

        productIds = parsed;
        return true;
    }

    private static bool TryReadEnum<TEnum>(JsonElement root, string propertyName, out TEnum value)
        where TEnum : struct, Enum
    {
        value = default;
        return root.TryGetProperty(propertyName, out var element)
            && element.ValueKind == JsonValueKind.String
            && Enum.TryParse(element.GetString(), true, out value)
            && Enum.IsDefined(value);
    }

    private static bool TryReadNullableEnum<TEnum>(JsonElement root, string propertyName, out TEnum? value)
        where TEnum : struct, Enum
    {
        value = null;
        if (!root.TryGetProperty(propertyName, out var element))
        {
            return false;
        }

        if (element.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        if (element.ValueKind != JsonValueKind.String
            || !Enum.TryParse(element.GetString(), true, out TEnum parsed)
            || !Enum.IsDefined(parsed))
        {
            return false;
        }

        value = parsed;
        return true;
    }

    private static bool TryReadNullableString(
        JsonElement root,
        string propertyName,
        int maximumLength,
        out string? value)
    {
        value = null;
        if (!root.TryGetProperty(propertyName, out var element))
        {
            return false;
        }

        if (element.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        value = element.GetString();
        return value is null || value.Length <= maximumLength;
    }

    private static bool TryReadRequiredString(
        JsonElement root,
        string propertyName,
        int maximumLength,
        out string value)
    {
        value = string.Empty;
        if (!root.TryGetProperty(propertyName, out var element)
            || element.ValueKind != JsonValueKind.String
            || string.IsNullOrWhiteSpace(element.GetString()))
        {
            return false;
        }

        value = element.GetString()!.Trim();
        return value.Length <= maximumLength;
    }

    private static bool TryReadNullableDecimal(JsonElement root, string propertyName, out decimal? value)
    {
        value = null;
        if (!root.TryGetProperty(propertyName, out var element))
        {
            return false;
        }

        if (element.ValueKind == JsonValueKind.Null)
        {
            return true;
        }

        if (element.ValueKind != JsonValueKind.Number || !element.TryGetDecimal(out var parsed) || parsed < 0)
        {
            return false;
        }

        value = parsed;
        return true;
    }

    private static bool TryReadBoolean(JsonElement root, string propertyName, out bool value)
    {
        value = false;
        if (!root.TryGetProperty(propertyName, out var element)
            || element.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
        {
            return false;
        }

        value = element.GetBoolean();
        return true;
    }
}

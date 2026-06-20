using System.Text.Json;

namespace Ecommerce.Api.Assistant;

public sealed class AssistantIntentPlanJsonParser
{
    public AssistantIntentPlan? Parse(
        string? json,
        int maxResponseCharacters)
    {
        if (string.IsNullOrWhiteSpace(json)
            || json.Length > maxResponseCharacters)
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(json);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            if (!TryReadKind(document.RootElement, out var kind)
                || !TryReadTools(document.RootElement, out var tools)
                || !TryReadArguments(document.RootElement, out var arguments))
            {
                return null;
            }

            return new AssistantIntentPlan(kind, tools, arguments);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool TryReadKind(
        JsonElement root,
        out AssistantIntentKind kind)
    {
        kind = AssistantIntentKind.Unsupported;

        if (!root.TryGetProperty("kind", out var kindElement)
            || kindElement.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        return Enum.TryParse(kindElement.GetString(), ignoreCase: true, out kind);
    }

    private static bool TryReadTools(
        JsonElement root,
        out IReadOnlyCollection<string> tools)
    {
        tools = Array.Empty<string>();

        if (!root.TryGetProperty("tools", out var toolsElement)
            || toolsElement.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        var parsedTools = new List<string>();

        foreach (var toolElement in toolsElement.EnumerateArray())
        {
            if (toolElement.ValueKind != JsonValueKind.String
                || string.IsNullOrWhiteSpace(toolElement.GetString()))
            {
                return false;
            }

            parsedTools.Add(toolElement.GetString()!);
        }

        tools = parsedTools;
        return true;
    }

    private static bool TryReadArguments(
        JsonElement root,
        out IReadOnlyDictionary<string, string?> arguments)
    {
        arguments = AssistantIntentPlan.EmptyArguments();

        if (!root.TryGetProperty("arguments", out var argumentsElement))
        {
            return true;
        }

        if (argumentsElement.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        var parsedArguments = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var argument in argumentsElement.EnumerateObject())
        {
            if (string.IsNullOrWhiteSpace(argument.Name)
                || !TryReadArgumentValue(argument.Value, out var value))
            {
                return false;
            }

            parsedArguments[argument.Name] = value;
        }

        arguments = parsedArguments;
        return true;
    }

    private static bool TryReadArgumentValue(
        JsonElement element,
        out string? value)
    {
        value = null;

        switch (element.ValueKind)
        {
            case JsonValueKind.Null:
                return true;
            case JsonValueKind.String:
                value = element.GetString();
                return true;
            case JsonValueKind.Number:
                value = element.GetRawText();
                return true;
            case JsonValueKind.True:
            case JsonValueKind.False:
                value = element.GetBoolean() ? "true" : "false";
                return true;
            default:
                return false;
        }
    }
}

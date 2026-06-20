using System.Globalization;

namespace Ecommerce.Api.Assistant;

public sealed record AssistantIntentPlan(
    AssistantIntentKind Kind,
    IReadOnlyCollection<string> Tools,
    IReadOnlyDictionary<string, string?> Arguments)
{
    public static AssistantIntentPlan Unsupported() =>
        new(AssistantIntentKind.Unsupported, Array.Empty<string>(), EmptyArguments());

    public static AssistantIntentPlan FromIntent(AssistantIntent intent)
    {
        var arguments = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(intent.SearchText))
        {
            arguments["searchText"] = intent.SearchText;
        }

        if (intent.Amount.HasValue)
        {
            arguments["amount"] = intent.Amount.Value.ToString(CultureInfo.InvariantCulture);
        }

        if (intent.ProductId.HasValue)
        {
            arguments["productId"] = intent.ProductId.Value.ToString();
        }

        return new AssistantIntentPlan(
            intent.Kind,
            AssistantIntentToolPlan.GetExpectedTools(intent.Kind),
            arguments);
    }

    public static IReadOnlyDictionary<string, string?> EmptyArguments() =>
        new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
}

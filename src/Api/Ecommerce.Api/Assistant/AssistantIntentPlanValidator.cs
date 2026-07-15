using System.Globalization;

namespace Ecommerce.Api.Assistant;

public sealed class AssistantIntentPlanValidator(
    AssistantToolRegistry toolRegistry,
    AssistantSafetyPolicy safetyPolicy)
{
    private const int MaxArgumentLength = 200;

    public AssistantIntent Validate(string question, AssistantIntentPlan? plan)
    {
        return ValidateWithDiagnostics(question, plan).Intent;
    }

    public AssistantIntentPlanValidationResult ValidateWithDiagnostics(string question, AssistantIntentPlan? plan)
    {
        if (string.IsNullOrWhiteSpace(question)
            || plan is null
            || safetyPolicy.IsUnsafeQuestion(question)
            || !Enum.IsDefined(plan.Kind)
            || !ToolsMatchIntent(plan.Kind, plan.Tools)
            || !ArgumentsAreAllowed(plan.Kind, plan.Arguments))
        {
            return new AssistantIntentPlanValidationResult(Unsupported(), plan is not null);
        }

        var intent = plan.Kind switch
        {
            AssistantIntentKind.Unsupported => Unsupported(),
            AssistantIntentKind.RecentOrders => new AssistantIntent(AssistantIntentKind.RecentOrders),
            AssistantIntentKind.TotalSpend => new AssistantIntent(AssistantIntentKind.TotalSpend),
            AssistantIntentKind.ProductsOrdered => new AssistantIntent(AssistantIntentKind.ProductsOrdered),
            AssistantIntentKind.ProductFrequency => new AssistantIntent(AssistantIntentKind.ProductFrequency),
            AssistantIntentKind.OrdersAboveAmount => ValidateAmountIntent(plan, AssistantIntentKind.OrdersAboveAmount),
            AssistantIntentKind.OrdersContainingProductsOverAmount => ValidateAmountIntent(plan, AssistantIntentKind.OrdersContainingProductsOverAmount),
            AssistantIntentKind.CatalogSearchProducts => ValidateCatalogSearchProductsIntent(plan),
            AssistantIntentKind.CatalogGetProductBySearch => ValidateCatalogGetProductBySearchIntent(plan),
            AssistantIntentKind.CatalogProductsUnderPrice => ValidateAmountIntent(plan, AssistantIntentKind.CatalogProductsUnderPrice),
            AssistantIntentKind.OrdersContainingProduct => ValidateProductSearchIntent(plan),
            AssistantIntentKind.CatalogGetProduct => ValidateCatalogGetProductIntent(plan),
            _ => Unsupported()
        };

        var failedValidation = plan.Kind != AssistantIntentKind.Unsupported
            && intent.Kind == AssistantIntentKind.Unsupported;

        return new AssistantIntentPlanValidationResult(intent, failedValidation);
    }

    private bool ToolsMatchIntent(
        AssistantIntentKind kind,
        IReadOnlyCollection<string>? proposedTools)
    {
        var expectedTools = AssistantIntentToolPlan
            .GetExpectedTools(kind)
            .OrderBy(tool => tool, StringComparer.Ordinal)
            .ToArray();
        var tools = (proposedTools ?? Array.Empty<string>())
            .OrderBy(tool => tool, StringComparer.Ordinal)
            .ToArray();

        if (tools.Any(tool => string.IsNullOrWhiteSpace(tool) || !toolRegistry.IsAllowed(tool)))
        {
            return false;
        }

        return tools.SequenceEqual(expectedTools, StringComparer.Ordinal);
    }

    private bool ArgumentsAreAllowed(
        AssistantIntentKind kind,
        IReadOnlyDictionary<string, string?>? arguments)
    {
        var argumentMap = arguments ?? AssistantIntentPlan.EmptyArguments();

        return argumentMap.All(argument =>
            !safetyPolicy.IsForbiddenArgumentName(argument.Key)
            && IsAllowedArgument(kind, argument.Key)
            && (argument.Value?.Length ?? 0) <= MaxArgumentLength);
    }

    private static bool IsAllowedArgument(AssistantIntentKind kind, string argumentName) =>
        kind switch
        {
            AssistantIntentKind.OrdersAboveAmount
                or AssistantIntentKind.OrdersContainingProductsOverAmount
                or AssistantIntentKind.CatalogProductsUnderPrice
                => string.Equals(argumentName, "amount", StringComparison.OrdinalIgnoreCase),
            AssistantIntentKind.OrdersContainingProduct
                => string.Equals(argumentName, "searchText", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(argumentName, "productId", StringComparison.OrdinalIgnoreCase),
            AssistantIntentKind.CatalogSearchProducts
                or AssistantIntentKind.CatalogGetProductBySearch
                => string.Equals(argumentName, "searchText", StringComparison.OrdinalIgnoreCase),
            AssistantIntentKind.CatalogGetProduct
                => string.Equals(argumentName, "productId", StringComparison.OrdinalIgnoreCase),
            _ => false
        };

    private static AssistantIntent ValidateAmountIntent(
        AssistantIntentPlan plan,
        AssistantIntentKind kind)
    {
        if (!TryGetArgument(plan, "amount", out var rawAmount)
            || !decimal.TryParse(
                rawAmount,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var amount)
            || amount < 0)
        {
            return Unsupported();
        }

        return new AssistantIntent(kind, Amount: amount);
    }

    private AssistantIntent ValidateCatalogSearchProductsIntent(AssistantIntentPlan plan)
    {
        if (!TryGetArgument(plan, "searchText", out var rawSearchText)
            || safetyPolicy.IsUnsafeQuestion(rawSearchText))
        {
            return Unsupported();
        }

        return new AssistantIntent(AssistantIntentKind.CatalogSearchProducts, SearchText: rawSearchText);
    }

    private AssistantIntent ValidateCatalogGetProductBySearchIntent(AssistantIntentPlan plan)
    {
        if (!TryGetArgument(plan, "searchText", out var rawSearchText)
            || safetyPolicy.IsUnsafeQuestion(rawSearchText))
        {
            return Unsupported();
        }

        return new AssistantIntent(AssistantIntentKind.CatalogGetProductBySearch, SearchText: rawSearchText);
    }

    private static AssistantIntent ValidateProductSearchIntent(AssistantIntentPlan plan)
    {
        Guid? productId = null;
        string? searchText = null;

        if (TryGetArgument(plan, "productId", out var rawProductId))
        {
            if (!Guid.TryParse(rawProductId, out var parsedProductId))
            {
                return Unsupported();
            }

            productId = parsedProductId;
            searchText = rawProductId;
        }

        if (TryGetArgument(plan, "searchText", out var rawSearchText))
        {
            searchText = rawSearchText;
        }

        return string.IsNullOrWhiteSpace(searchText)
            ? Unsupported()
            : new AssistantIntent(
                AssistantIntentKind.OrdersContainingProduct,
                SearchText: searchText,
                ProductId: productId);
    }

    private static AssistantIntent ValidateCatalogGetProductIntent(AssistantIntentPlan plan)
    {
        if (!TryGetArgument(plan, "productId", out var rawProductId)
            || !Guid.TryParse(rawProductId, out var productId))
        {
            return Unsupported();
        }

        return new AssistantIntent(AssistantIntentKind.CatalogGetProduct, ProductId: productId);
    }

    private static bool TryGetArgument(
        AssistantIntentPlan plan,
        string argumentName,
        out string value)
    {
        var arguments = plan.Arguments ?? AssistantIntentPlan.EmptyArguments();

        if (arguments.TryGetValue(argumentName, out var rawValue)
            && !string.IsNullOrWhiteSpace(rawValue))
        {
            value = rawValue.Trim();
            return true;
        }

        value = string.Empty;
        return false;
    }

    private static AssistantIntent Unsupported() =>
        new(AssistantIntentKind.Unsupported);
}

public sealed record AssistantIntentPlanValidationResult(
    AssistantIntent Intent,
    bool ModelOutputFailedValidation);

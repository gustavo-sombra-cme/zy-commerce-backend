using System.Globalization;
using System.Text.RegularExpressions;

namespace Ecommerce.Api.Assistant;

public sealed class AssistantIntentRouter
{
    private static readonly Regex AmountRegex = new(
        @"(?<amount>\d+(?:\.\d{1,2})?)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private readonly AssistantSafetyPolicy safetyPolicy;

    public AssistantIntentRouter()
        : this(new AssistantSafetyPolicy())
    {
    }

    public AssistantIntentRouter(AssistantSafetyPolicy safetyPolicy)
    {
        this.safetyPolicy = safetyPolicy;
    }

    public AssistantIntent Route(string question)
    {
        var raw = question.Trim();
        var normalized = Normalize(question);

        if (string.IsNullOrWhiteSpace(normalized) || safetyPolicy.IsUnsafeQuestion(normalized))
        {
            return new AssistantIntent(AssistantIntentKind.Unsupported);
        }

        if (normalized.Contains("recent orders", StringComparison.Ordinal)
            || normalized.Contains("latest orders", StringComparison.Ordinal)
            || normalized.Contains("show my orders", StringComparison.Ordinal))
        {
            return new AssistantIntent(AssistantIntentKind.RecentOrders);
        }

        if (normalized.Contains("total spend", StringComparison.Ordinal)
            || normalized.Contains("how much did i spend", StringComparison.Ordinal)
            || normalized.Contains("total amount", StringComparison.Ordinal))
        {
            return new AssistantIntent(AssistantIntentKind.TotalSpend);
        }

        if (normalized.Contains("buy most often", StringComparison.Ordinal)
            || normalized.Contains("bought most often", StringComparison.Ordinal)
            || normalized.Contains("most often", StringComparison.Ordinal)
            || normalized.Contains("product frequency", StringComparison.Ordinal))
        {
            return new AssistantIntent(AssistantIntentKind.ProductFrequency);
        }

        if (normalized.Contains("products did i order", StringComparison.Ordinal)
            || normalized.Contains("products i ordered", StringComparison.Ordinal)
            || normalized.Contains("what did i buy", StringComparison.Ordinal))
        {
            return new AssistantIntent(AssistantIntentKind.ProductsOrdered);
        }

        if (normalized.Contains("products", StringComparison.Ordinal)
            && HasUnderWord(normalized)
            && TryParseAmount(normalized, out var productAmount))
        {
            return new AssistantIntent(AssistantIntentKind.CatalogProductsUnderPrice, Amount: productAmount);
        }

        if (normalized.Contains("orders", StringComparison.Ordinal)
            && normalized.Contains("products", StringComparison.Ordinal)
            && HasOverWord(normalized)
            && TryParseAmount(normalized, out var lineAmount))
        {
            return new AssistantIntent(AssistantIntentKind.OrdersContainingProductsOverAmount, Amount: lineAmount);
        }

        if (normalized.Contains("orders", StringComparison.Ordinal)
            && HasOverWord(normalized)
            && TryParseAmount(normalized, out var orderAmount))
        {
            return new AssistantIntent(AssistantIntentKind.OrdersAboveAmount, Amount: orderAmount);
        }

        if (normalized.Contains("orders", StringComparison.Ordinal)
            && (normalized.Contains("contain", StringComparison.Ordinal)
                || normalized.Contains("containing", StringComparison.Ordinal)
                || normalized.Contains("with", StringComparison.Ordinal))
            && TryExtractProductSearch(normalized, out var searchText))
        {
            return new AssistantIntent(
                AssistantIntentKind.OrdersContainingProduct,
                SearchText: searchText,
                ProductId: Guid.TryParse(searchText, out var productId) ? productId : null);
        }

        if (normalized.Contains("product", StringComparison.Ordinal)
            && Guid.TryParse(ExtractLastToken(normalized), out var parsedProductId))
        {
            return new AssistantIntent(AssistantIntentKind.CatalogGetProduct, ProductId: parsedProductId);
        }

        if (TryExtractCatalogDetailSearch(raw, normalized, out var detailSearchText))
        {
            return new AssistantIntent(
                AssistantIntentKind.CatalogGetProductBySearch,
                SearchText: detailSearchText);
        }

        if (TryExtractCatalogSearch(raw, normalized, out var catalogSearchText))
        {
            return new AssistantIntent(
                AssistantIntentKind.CatalogSearchProducts,
                SearchText: catalogSearchText);
        }

        return new AssistantIntent(AssistantIntentKind.Unsupported);
    }

    private static bool TryParseAmount(string question, out decimal amount)
    {
        var match = AmountRegex.Match(question);
        return decimal.TryParse(
            match.Success ? match.Groups["amount"].Value : string.Empty,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out amount);
    }

    private static bool TryExtractProductSearch(string question, out string searchText)
    {
        var markers = new[] { "product/sku/name", "product id", "product", "sku", "name" };
        var selectedMarker = string.Empty;
        var selectedIndex = -1;

        foreach (var marker in markers)
        {
            var index = question.LastIndexOf(marker, StringComparison.Ordinal);

            if (index <= selectedIndex)
            {
                continue;
            }

            selectedMarker = marker;
            selectedIndex = index;
        }

        if (selectedIndex >= 0)
        {
            searchText = question[(selectedIndex + selectedMarker.Length)..]
                .Trim(' ', ':', '#', '/', '.', '?', '!', ',', ';', '"', '\'');

            return !string.IsNullOrWhiteSpace(searchText);
        }

        searchText = ExtractLastToken(question);
        return !string.IsNullOrWhiteSpace(searchText);
    }

    private static string ExtractLastToken(string question)
    {
        var tokens = question.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return tokens.Length == 0 ? string.Empty : tokens[^1].Trim('.', '?', '!', ',', ':', ';', '"', '\'');
    }

    private static bool TryExtractCatalogSearch(
        string question,
        string normalized,
        out string searchText)
    {
        searchText = string.Empty;

        if (!LooksLikeCatalogSearch(normalized))
        {
            return false;
        }

        var candidate = question.Trim(' ', '.', '?', '!', ',', ':', ';', '"', '\'');
        candidate = RemoveLeadingPhrase(candidate, "show active products matching");
        candidate = RemoveLeadingPhrase(candidate, "show products matching");
        candidate = RemoveLeadingPhrase(candidate, "products matching");
        candidate = RemoveLeadingPhrase(candidate, "product matching");
        candidate = RemoveLeadingPhrase(candidate, "search for SKU");
        candidate = RemoveLeadingPhrase(candidate, "search for");
        candidate = RemoveLeadingPhrase(candidate, "find SKU");
        candidate = RemoveLeadingPhrase(candidate, "find");
        candidate = RemoveLeadingPhrase(candidate, "show me");
        candidate = RemoveLeadingPhrase(candidate, "show active products");
        candidate = RemoveLeadingPhrase(candidate, "show products");
        candidate = RemoveLeadingPhrase(candidate, "show");
        candidate = RemoveLeadingPhrase(candidate, "do you have");
        candidate = RemoveLeadingPhrase(candidate, "do you carry");
        candidate = RemoveLeadingPhrase(candidate, "SKU");
        candidate = RemoveTrailingWord(candidate, "products");
        candidate = RemoveTrailingWord(candidate, "product");

        searchText = candidate.Trim(' ', '.', '?', '!', ',', ':', ';', '"', '\'');
        return !string.IsNullOrWhiteSpace(searchText);
    }

    private static bool TryExtractCatalogDetailSearch(
        string question,
        string normalized,
        out string searchText)
    {
        searchText = string.Empty;

        var leadingPhrase = normalized switch
        {
            _ when normalized.StartsWith("show me details for ", StringComparison.Ordinal) => "show me details for",
            _ when normalized.StartsWith("show details for ", StringComparison.Ordinal) => "show details for",
            _ when normalized.StartsWith("details for ", StringComparison.Ordinal) => "details for",
            _ when normalized.StartsWith("tell me about ", StringComparison.Ordinal) => "tell me about",
            _ when normalized.StartsWith("what is the price of ", StringComparison.Ordinal) => "what is the price of",
            _ when normalized.StartsWith("how much is ", StringComparison.Ordinal) => "how much is",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(leadingPhrase))
        {
            return false;
        }

        var candidate = question.Trim(' ', '.', '?', '!', ',', ':', ';', '"', '\'');
        candidate = RemoveLeadingPhrase(candidate, leadingPhrase);
        candidate = RemoveLeadingPhrase(candidate, "SKU");

        searchText = candidate.Trim(' ', '.', '?', '!', ',', ':', ';', '"', '\'');
        return !string.IsNullOrWhiteSpace(searchText);
    }

    private static bool LooksLikeCatalogSearch(string normalized) =>
        normalized.Contains("sku", StringComparison.Ordinal)
        || normalized.Contains("products matching", StringComparison.Ordinal)
        || normalized.Contains("product matching", StringComparison.Ordinal)
        || normalized.StartsWith("search for ", StringComparison.Ordinal)
        || normalized.StartsWith("do you have ", StringComparison.Ordinal)
        || normalized.StartsWith("do you carry ", StringComparison.Ordinal)
        || normalized.StartsWith("show active products matching ", StringComparison.Ordinal)
        || (normalized.Contains("product", StringComparison.Ordinal)
            && (normalized.StartsWith("show me ", StringComparison.Ordinal)
                || normalized.StartsWith("find ", StringComparison.Ordinal)
                || normalized.StartsWith("show ", StringComparison.Ordinal)));

    private static string RemoveLeadingPhrase(string value, string phrase)
    {
        if (!value.StartsWith(phrase, StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        return value[phrase.Length..].Trim();
    }

    private static string RemoveTrailingWord(string value, string word)
    {
        if (!value.EndsWith(word, StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        return value[..^word.Length].Trim();
    }

    private static bool HasUnderWord(string question) =>
        question.Contains("under", StringComparison.Ordinal)
        || question.Contains("below", StringComparison.Ordinal)
        || question.Contains("less than", StringComparison.Ordinal);

    private static bool HasOverWord(string question) =>
        question.Contains("over", StringComparison.Ordinal)
        || question.Contains("above", StringComparison.Ordinal)
        || question.Contains("greater than", StringComparison.Ordinal)
        || question.Contains("more than", StringComparison.Ordinal);

    private static string Normalize(string question) =>
        question.Trim().ToLowerInvariant();
}

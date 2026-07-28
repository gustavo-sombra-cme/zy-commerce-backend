using System.Text.RegularExpressions;

namespace Ecommerce.Api.Assistant;

internal enum CatalogComparisonMode
{
    Compare,
    Cheaper
}

internal sealed record CatalogComparisonRequest(
    string FirstSearchText,
    string SecondSearchText,
    CatalogComparisonMode Mode);

internal static partial class CatalogComparisonRequestParser
{
    private const int MaximumSearchTextLength = 200;

    public static bool TryParse(string question, out CatalogComparisonRequest request)
    {
        request = null!;
        if (string.IsNullOrWhiteSpace(question))
        {
            return false;
        }

        var match = CompareRegex().Match(question);
        var mode = CatalogComparisonMode.Compare;
        if (!match.Success)
        {
            match = CheaperRegex().Match(question);
            mode = CatalogComparisonMode.Cheaper;
        }

        if (!match.Success)
        {
            match = DifferenceRegex().Match(question);
        }

        if (!match.Success
            || !TryNormalizeSearchText(match.Groups["first"].Value, out var first)
            || !TryNormalizeSearchText(match.Groups["second"].Value, out var second))
        {
            return false;
        }

        request = new CatalogComparisonRequest(first, second, mode);
        return true;
    }

    private static bool TryNormalizeSearchText(string value, out string searchText)
    {
        var normalized = value.Trim(' ', '.', '?', '!', ',', ':', ';', '"', '\'');
        if (normalized.StartsWith("SKU ", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[4..].Trim();
        }

        searchText = normalized;
        return searchText.Length is > 0 and <= MaximumSearchTextLength;
    }

    [GeneratedRegex(
        @"^\s*compare\s+(?<first>.+?)\s+(?:and|with)\s+(?<second>.+?)\s*[?.!]*\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex CompareRegex();

    [GeneratedRegex(
        @"^\s*which\s+is\s+cheaper\s*,?\s*(?<first>.+?)\s+or\s+(?<second>.+?)\s*[?.!]*\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex CheaperRegex();

    [GeneratedRegex(
        @"^\s*what\s+is\s+the\s+difference\s+between\s+(?<first>.+?)\s+and\s+(?<second>.+?)\s*[?.!]*\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex DifferenceRegex();
}

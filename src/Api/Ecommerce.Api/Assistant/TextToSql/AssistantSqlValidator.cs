using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant.TextToSql;

public sealed partial class AssistantSqlValidator(IOptions<AssistantTextToSqlOptions> options)
{
    private static readonly IReadOnlyDictionary<AssistantSqlDataSource, string[]> AllowedViews =
        new Dictionary<AssistantSqlDataSource, string[]>
        {
            [AssistantSqlDataSource.Catalog] =
            [
                "assistant.v_productsearch",
                "assistant.v_productdetails"
            ],
            [AssistantSqlDataSource.Orders] =
            [
                "assistant.v_myorders",
                "assistant.v_myorderlines",
                "assistant.v_myordersummary"
            ]
        };

    public AssistantSqlValidationResult Validate(AssistantSqlQuery query)
    {
        if (!Enum.IsDefined(query.DataSource))
        {
            return AssistantSqlValidationResult.Invalid("Unsupported data source.");
        }

        if (string.IsNullOrWhiteSpace(query.Sql))
        {
            return AssistantSqlValidationResult.Invalid("SQL is required.");
        }

        var sql = query.Sql.Trim();
        var normalized = Normalize(sql);

        if (ContainsForbiddenSyntax(sql, normalized))
        {
            return AssistantSqlValidationResult.Invalid("SQL contains forbidden syntax.");
        }

        var topMatch = SelectTopRegex().Match(sql);
        if (!topMatch.Success)
        {
            return AssistantSqlValidationResult.Invalid("SQL must be a SELECT TOP query.");
        }

        if (!int.TryParse(topMatch.Groups["top"].Value, out var top)
            || top <= 0
            || top > options.Value.EffectiveMaxRows)
        {
            return AssistantSqlValidationResult.Invalid("SQL TOP value exceeds the configured maximum.");
        }

        var referencedViews = ReferencedObjectRegex()
            .Matches(sql)
            .Select(match => NormalizeObjectName(match.Groups["object"].Value))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (referencedViews.Length == 0)
        {
            return AssistantSqlValidationResult.Invalid("SQL must reference an approved assistant view.");
        }

        if (ReferencesForbiddenObjects(referencedViews)
            || !ReferencesOnlyAllowedViews(query.DataSource, referencedViews))
        {
            return AssistantSqlValidationResult.Invalid("SQL references an unapproved object.");
        }

        if (query.DataSource == AssistantSqlDataSource.Orders
            && !HasCurrentUserScope(normalized))
        {
            return AssistantSqlValidationResult.Invalid("Orders SQL must be scoped to @CurrentUserId.");
        }

        if (query.DataSource == AssistantSqlDataSource.Catalog
            && normalized.Contains("buyeruserid", StringComparison.Ordinal))
        {
            return AssistantSqlValidationResult.Invalid("Catalog SQL must not include order ownership scope.");
        }

        return AssistantSqlValidationResult.Valid();
    }

    private static bool ContainsForbiddenSyntax(string sql, string normalized)
    {
        if (sql.Contains(';')
            || sql.Contains("--", StringComparison.Ordinal)
            || sql.Contains("/*", StringComparison.Ordinal)
            || sql.Contains("*/", StringComparison.Ordinal))
        {
            return true;
        }

        return ForbiddenTokenRegex().IsMatch(normalized)
            || normalized.Contains(" union ", StringComparison.Ordinal)
            || normalized.Contains("#", StringComparison.Ordinal)
            || normalized.Contains("@@", StringComparison.Ordinal)
            || normalized.Contains("sp_", StringComparison.Ordinal)
            || normalized.Contains("exec(", StringComparison.Ordinal)
            || normalized.Contains("execute(", StringComparison.Ordinal)
            || normalized.Contains("openrowset", StringComparison.Ordinal)
            || normalized.Contains("xp_cmdshell", StringComparison.Ordinal)
            || normalized.Contains("information_schema", StringComparison.Ordinal)
            || normalized.Contains("sys.", StringComparison.Ordinal);
    }

    private static bool ReferencesForbiddenObjects(IEnumerable<string> objectNames)
    {
        return objectNames.Any(name =>
            name.StartsWith("dbo.", StringComparison.Ordinal)
            || name.StartsWith("catalog.", StringComparison.Ordinal)
            || name.StartsWith("orders.", StringComparison.Ordinal)
            || name.StartsWith("auth.", StringComparison.Ordinal)
            || name.StartsWith("sys.", StringComparison.Ordinal)
            || name.StartsWith("information_schema.", StringComparison.Ordinal));
    }

    private static bool ReferencesOnlyAllowedViews(
        AssistantSqlDataSource dataSource,
        IReadOnlyCollection<string> objectNames)
    {
        var allowed = AllowedViews[dataSource].ToHashSet(StringComparer.Ordinal);
        return objectNames.All(allowed.Contains);
    }

    private static bool HasCurrentUserScope(string normalized)
    {
        return CurrentUserScopeRegex().IsMatch(normalized);
    }

    private static string Normalize(string sql) =>
        WhitespaceRegex().Replace(sql, " ").Trim().ToLowerInvariant();

    private static string NormalizeObjectName(string objectName) =>
        objectName.Replace("[", string.Empty, StringComparison.Ordinal)
            .Replace("]", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();

    [GeneratedRegex(@"^\s*select\s+top\s*\(?\s*(?<top>\d+)\s*\)?\s+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SelectTopRegex();

    [GeneratedRegex(@"\b(?:from|join)\s+(?<object>(?:\[[A-Za-z_][A-Za-z0-9_]*\]|[A-Za-z_][A-Za-z0-9_]*)\s*\.\s*(?:\[[A-Za-z_][A-Za-z0-9_]*\]|[A-Za-z_][A-Za-z0-9_]*))", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ReferencedObjectRegex();

    [GeneratedRegex(@"\b(insert|update|delete|merge|create|alter|drop|truncate|exec|execute|grant|revoke|deny|declare|set)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ForbiddenTokenRegex();

    [GeneratedRegex(@"\bbuyeruserid\s*=\s*@currentuserid\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex CurrentUserScopeRegex();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespaceRegex();
}

using System.Text.Json;

namespace Ecommerce.Api.Assistant.TextToSql;

public sealed class AssistantTextToSqlPlanParser
{
    public AssistantTextToSqlPlan Parse(string? json, int maxCharacters)
    {
        if (string.IsNullOrWhiteSpace(json)
            || json.Length > maxCharacters)
        {
            return AssistantTextToSqlPlan.Unsupported("Planner output was empty or too large.");
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("supported", out var supportedElement)
                || supportedElement.ValueKind is not JsonValueKind.True and not JsonValueKind.False
                || !root.TryGetProperty("resultShape", out var resultShapeElement)
                || resultShapeElement.ValueKind != JsonValueKind.String)
            {
                return AssistantTextToSqlPlan.Unsupported("Planner output did not match the expected shape.");
            }

            var supported = supportedElement.GetBoolean();
            var resultShape = ParseResultShape(resultShapeElement.GetString());
            if (resultShape is null)
            {
                return AssistantTextToSqlPlan.Unsupported("Planner output used an unsupported result shape.");
            }

            var reason = ReadNullableString(root, "reason");

            if (!supported)
            {
                return UnsupportedPlanIsConsistent(root, resultShape.Value)
                    ? AssistantTextToSqlPlan.Unsupported(reason)
                    : AssistantTextToSqlPlan.Unsupported("Unsupported planner output was inconsistent.");
            }

            if (resultShape == AssistantTextToSqlResultShape.Unsupported)
            {
                return AssistantTextToSqlPlan.Unsupported("Supported planner output cannot use unsupported result shape.");
            }

            var dataSource = ParseDataSource(ReadNullableString(root, "dataSource"));
            var sql = ReadNullableString(root, "sql");

            if (dataSource is null || string.IsNullOrWhiteSpace(sql))
            {
                return AssistantTextToSqlPlan.Unsupported("Supported planner output was missing data source or SQL.");
            }

            return new AssistantTextToSqlPlan(
                true,
                dataSource,
                sql.Trim(),
                resultShape.Value,
                reason);
        }
        catch (JsonException)
        {
            return AssistantTextToSqlPlan.Unsupported("Planner output was not valid JSON.");
        }
    }

    private static bool UnsupportedPlanIsConsistent(
        JsonElement root,
        AssistantTextToSqlResultShape resultShape)
    {
        return resultShape == AssistantTextToSqlResultShape.Unsupported
            && IsNullOrMissing(root, "dataSource")
            && IsNullOrMissing(root, "sql");
    }

    private static bool IsNullOrMissing(JsonElement root, string propertyName)
    {
        return !root.TryGetProperty(propertyName, out var value)
            || value.ValueKind == JsonValueKind.Null;
    }

    private static string? ReadNullableString(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var value)
            || value.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static AssistantSqlDataSource? ParseDataSource(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            "catalog" => AssistantSqlDataSource.Catalog,
            "orders" => AssistantSqlDataSource.Orders,
            _ => null
        };
    }

    private static AssistantTextToSqlResultShape? ParseResultShape(string? value)
    {
        return value?.Trim() switch
        {
            "productList" => AssistantTextToSqlResultShape.ProductList,
            "productDetails" => AssistantTextToSqlResultShape.ProductDetails,
            "orderList" => AssistantTextToSqlResultShape.OrderList,
            "orderDetails" => AssistantTextToSqlResultShape.OrderDetails,
            "spendSummary" => AssistantTextToSqlResultShape.SpendSummary,
            "genericTable" => AssistantTextToSqlResultShape.GenericTable,
            "unsupported" => AssistantTextToSqlResultShape.Unsupported,
            _ => null
        };
    }
}

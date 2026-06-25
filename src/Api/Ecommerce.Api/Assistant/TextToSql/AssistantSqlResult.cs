namespace Ecommerce.Api.Assistant.TextToSql;

public sealed record AssistantSqlResult(
    bool Succeeded,
    IReadOnlyList<string> Columns,
    IReadOnlyList<AssistantSqlRow> Rows,
    int RowCount,
    bool Truncated,
    string? Error = null)
{
    public static AssistantSqlResult Failure() =>
        new(false, Array.Empty<string>(), Array.Empty<AssistantSqlRow>(), 0, false, "SQL execution failed.");
}

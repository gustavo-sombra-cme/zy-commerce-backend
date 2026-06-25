namespace Ecommerce.Api.Assistant.TextToSql;

public sealed record AssistantTextToSqlPlan(
    bool Supported,
    AssistantSqlDataSource? DataSource,
    string? Sql,
    AssistantTextToSqlResultShape ResultShape,
    string? Reason)
{
    public static AssistantTextToSqlPlan Unsupported(string? reason = null) =>
        new(false, null, null, AssistantTextToSqlResultShape.Unsupported, reason ?? "Text-to-SQL planning is not supported for this request.");
}

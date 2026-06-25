namespace Ecommerce.Api.Assistant.TextToSql;

public sealed record AssistantSqlQuery(
    AssistantSqlDataSource DataSource,
    string Sql,
    Guid? CurrentUserId = null);

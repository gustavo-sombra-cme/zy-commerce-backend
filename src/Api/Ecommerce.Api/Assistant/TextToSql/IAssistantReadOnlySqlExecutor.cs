namespace Ecommerce.Api.Assistant.TextToSql;

public interface IAssistantReadOnlySqlExecutor
{
    Task<AssistantSqlResult> ExecuteAsync(
        AssistantSqlQuery query,
        CancellationToken cancellationToken);
}

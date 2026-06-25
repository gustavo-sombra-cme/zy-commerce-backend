using System.Data;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant.TextToSql;

public sealed class AssistantReadOnlySqlExecutor(
    AssistantSqlValidator validator,
    IAssistantSqlConnectionFactory connectionFactory,
    IOptions<AssistantTextToSqlOptions> options,
    ILogger<AssistantReadOnlySqlExecutor> logger) : IAssistantReadOnlySqlExecutor
{
    public async Task<AssistantSqlResult> ExecuteAsync(
        AssistantSqlQuery query,
        CancellationToken cancellationToken)
    {
        if (!options.Value.IsEnabled)
        {
            return AssistantSqlResult.Failure();
        }

        var validation = validator.Validate(query);
        if (!validation.IsValid)
        {
            return AssistantSqlResult.Failure();
        }

        if (query.DataSource == AssistantSqlDataSource.Orders
            && query.CurrentUserId is null)
        {
            return AssistantSqlResult.Failure();
        }

        try
        {
            await using var connection = connectionFactory.CreateConnection(query.DataSource);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = query.Sql;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = options.Value.EffectiveCommandTimeoutSeconds;

            if (query.DataSource == AssistantSqlDataSource.Orders)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@CurrentUserId";
                parameter.Value = query.CurrentUserId!.Value;
                command.Parameters.Add(parameter);
            }

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var columns = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .ToArray();
            var rows = new List<AssistantSqlRow>();
            var maxRows = options.Value.EffectiveMaxRows;
            var truncated = false;

            while (await reader.ReadAsync(cancellationToken))
            {
                if (rows.Count >= maxRows)
                {
                    truncated = true;
                    break;
                }

                var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var column in columns)
                {
                    var ordinal = reader.GetOrdinal(column);
                    values[column] = await reader.IsDBNullAsync(ordinal, cancellationToken)
                        ? null
                        : reader.GetValue(ordinal);
                }

                rows.Add(new AssistantSqlRow(values));
            }

            return new AssistantSqlResult(true, columns, rows, rows.Count, truncated);
        }
        catch (Exception exception) when (exception is InvalidOperationException or DataException or System.Data.Common.DbException)
        {
            logger.LogWarning(
                "Assistant Text-to-SQL read-only execution failed with {ExceptionType}.",
                exception.GetType().Name);

            return AssistantSqlResult.Failure();
        }
    }
}

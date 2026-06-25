using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Api.Assistant.TextToSql;

public sealed class AssistantSqlConnectionFactory(IConfiguration configuration) : IAssistantSqlConnectionFactory
{
    public const string CatalogConnectionName = "AssistantCatalogReadOnly";
    public const string OrdersConnectionName = "AssistantOrdersReadOnly";

    public DbConnection CreateConnection(AssistantSqlDataSource dataSource)
    {
        var connectionName = dataSource switch
        {
            AssistantSqlDataSource.Catalog => CatalogConnectionName,
            AssistantSqlDataSource.Orders => OrdersConnectionName,
            _ => throw new InvalidOperationException("Unsupported assistant SQL data source.")
        };

        var connectionString = configuration.GetConnectionString(connectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Assistant read-only connection string is not configured.");
        }

        return new SqlConnection(connectionString);
    }
}

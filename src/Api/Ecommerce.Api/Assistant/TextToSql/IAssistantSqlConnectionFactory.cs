using System.Data.Common;

namespace Ecommerce.Api.Assistant.TextToSql;

public interface IAssistantSqlConnectionFactory
{
    DbConnection CreateConnection(AssistantSqlDataSource dataSource);
}

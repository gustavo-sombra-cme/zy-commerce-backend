namespace Ecommerce.Api.Assistant.TextToSql;

public sealed record AssistantSqlRow(IReadOnlyDictionary<string, object?> Values);

namespace Ecommerce.Api.Assistant.TextToSql;

public sealed record AssistantSqlValidationResult(
    bool IsValid,
    string? Reason = null)
{
    public static AssistantSqlValidationResult Valid() => new(true);

    public static AssistantSqlValidationResult Invalid(string reason) => new(false, reason);
}

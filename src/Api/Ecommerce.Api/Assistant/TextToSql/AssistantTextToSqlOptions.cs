namespace Ecommerce.Api.Assistant.TextToSql;

public sealed class AssistantTextToSqlOptions
{
    public const string SectionName = "Assistant:TextToSql";

    public bool Enabled { get; init; }

    public int MaxRows { get; init; } = 50;

    public int CommandTimeoutSeconds { get; init; } = 5;

    public int EffectiveMaxRows => Math.Clamp(MaxRows, 1, 500);

    public int EffectiveCommandTimeoutSeconds => Math.Clamp(CommandTimeoutSeconds, 1, 30);

    public bool IsEnabled
    {
        get
        {
            var environmentValue = Environment.GetEnvironmentVariable("ECOMMERCE_ASSISTANT_TEXT_TO_SQL_ENABLED");
            return bool.TryParse(environmentValue, out var enabled)
                ? enabled
                : Enabled;
        }
    }
}

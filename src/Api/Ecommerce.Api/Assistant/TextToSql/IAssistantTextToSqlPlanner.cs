namespace Ecommerce.Api.Assistant.TextToSql;

public interface IAssistantTextToSqlPlanner
{
    Task<AssistantTextToSqlPlan> PlanAsync(
        string question,
        CancellationToken cancellationToken);
}

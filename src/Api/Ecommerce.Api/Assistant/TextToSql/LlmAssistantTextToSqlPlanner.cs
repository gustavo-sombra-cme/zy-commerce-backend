using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant.TextToSql;

public sealed class LlmAssistantTextToSqlPlanner(
    IAssistantLlmClient llmClient,
    AssistantTextToSqlPromptBuilder promptBuilder,
    AssistantTextToSqlPlanParser parser,
    IOptions<AssistantLlmOptions> llmOptions,
    ILogger<LlmAssistantTextToSqlPlanner> logger) : IAssistantTextToSqlPlanner
{
    public async Task<AssistantTextToSqlPlan> PlanAsync(
        string question,
        CancellationToken cancellationToken)
    {
        var options = llmOptions.Value;
        if (!options.Enabled)
        {
            logger.LogInformation(
                "Assistant Text-to-SQL planner diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                false,
                false);

            return AssistantTextToSqlPlan.Unsupported("LLM planning is disabled.");
        }

        using var timeout = new CancellationTokenSource(options.Timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeout.Token);

        try
        {
            var prompt = promptBuilder.BuildPrompt(question);
            var planJson = await llmClient.CreateIntentPlanJsonAsync(prompt, linked.Token);
            var providerCallFailed = planJson is null;

            logger.LogInformation(
                "Assistant Text-to-SQL planner diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                true,
                providerCallFailed);

            if (planJson is null)
            {
                return AssistantTextToSqlPlan.Unsupported("LLM planning failed.");
            }

            var plan = parser.Parse(planJson, options.MaxResponseCharacters);

            logger.LogInformation(
                "Assistant Text-to-SQL planner output diagnostics: plannerOutputSupported={PlannerOutputSupported}.",
                plan.Supported);

            return plan;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            logger.LogInformation(
                "Assistant Text-to-SQL planner diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                true,
                true);

            return AssistantTextToSqlPlan.Unsupported("LLM planning failed.");
        }
    }
}

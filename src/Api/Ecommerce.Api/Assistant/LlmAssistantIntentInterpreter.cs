using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class LlmAssistantIntentInterpreter(
    IAssistantLlmClient llmClient,
    AssistantIntentPlanJsonParser parser,
    IOptions<AssistantLlmOptions> options,
    ILogger<LlmAssistantIntentInterpreter> logger) : IAssistantIntentInterpreter
{
    public async Task<AssistantIntentPlan?> InterpretAsync(
        string question,
        CancellationToken cancellationToken)
    {
        var llmOptions = options.Value;

        if (!llmOptions.Enabled)
        {
            logger.LogInformation(
                "Assistant LLM provider diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                false,
                false);

            return null;
        }

        using var timeout = new CancellationTokenSource(llmOptions.Timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeout.Token);

        try
        {
            var planJson = await llmClient.CreateIntentPlanJsonAsync(
                question,
                linked.Token);

            var providerCallFailed = planJson is null;

            logger.LogInformation(
                "Assistant LLM provider diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                true,
                providerCallFailed);

            if (planJson is null)
            {
                return null;
            }

            var plan = parser.Parse(planJson, llmOptions.MaxResponseCharacters);

            logger.LogInformation(
                "Assistant LLM model output diagnostics: modelOutputFailedValidation={ModelOutputFailedValidation}.",
                plan is null);

            return plan;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            logger.LogInformation(
                "Assistant LLM provider diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                true,
                true);

            return null;
        }
    }
}

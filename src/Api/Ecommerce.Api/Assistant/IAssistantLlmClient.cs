namespace Ecommerce.Api.Assistant;

public interface IAssistantLlmClient
{
    Task<string?> CreateIntentPlanJsonAsync(
        string question,
        CancellationToken cancellationToken);
}

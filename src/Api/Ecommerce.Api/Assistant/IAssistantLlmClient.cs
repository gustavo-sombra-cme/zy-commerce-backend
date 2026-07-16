namespace Ecommerce.Api.Assistant;

public interface IAssistantLlmClient
{
    Task<string?> CreateIntentPlanJsonAsync(
        string question,
        CancellationToken cancellationToken);

    Task<string?> CreateAgentResponseJsonAsync(
        AssistantModelRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult<string?>(null);
}

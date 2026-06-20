namespace Ecommerce.Api.Assistant;

public interface IAssistantIntentInterpreter
{
    Task<AssistantIntentPlan?> InterpretAsync(
        string question,
        CancellationToken cancellationToken);
}

namespace Ecommerce.Api.Assistant;

public sealed class DeterministicAssistantIntentInterpreter(AssistantIntentRouter intentRouter)
    : IAssistantIntentInterpreter
{
    public Task<AssistantIntentPlan?> InterpretAsync(
        string question,
        CancellationToken cancellationToken)
    {
        var intent = intentRouter.Route(question);
        return Task.FromResult<AssistantIntentPlan?>(AssistantIntentPlan.FromIntent(intent));
    }
}

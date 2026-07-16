namespace Ecommerce.Api.Assistant;

public sealed class ProviderAssistantLanguageModel(
    IAssistantLlmClient llmClient,
    AssistantModelResponseJsonParser parser) : IAssistantLanguageModel
{
    public async Task<AssistantModelResponse> CompleteAsync(
        AssistantModelRequest request,
        CancellationToken cancellationToken)
    {
        var json = await llmClient.CreateAgentResponseJsonAsync(request, cancellationToken);
        return parser.Parse(json) ?? AssistantModelResponse.Failed();
    }
}

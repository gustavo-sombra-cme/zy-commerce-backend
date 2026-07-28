namespace Ecommerce.Api.Assistant;

public interface IAssistantLanguageModel
{
    Task<AssistantModelResponse> CompleteAsync(
        AssistantModelRequest request,
        CancellationToken cancellationToken);
}

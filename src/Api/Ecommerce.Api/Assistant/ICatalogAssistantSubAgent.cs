namespace Ecommerce.Api.Assistant;

public interface ICatalogAssistantSubAgent
{
    Task<AssistantQueryResponse> HandleAsync(
        AssistantIntent intent,
        CancellationToken cancellationToken);
}

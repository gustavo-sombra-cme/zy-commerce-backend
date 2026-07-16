namespace Ecommerce.Api.Assistant;

public interface ICatalogAssistantSubAgent
{
    Task<AssistantQueryResponse> RunAsync(
        CatalogAgentRequest request,
        CancellationToken cancellationToken);
}

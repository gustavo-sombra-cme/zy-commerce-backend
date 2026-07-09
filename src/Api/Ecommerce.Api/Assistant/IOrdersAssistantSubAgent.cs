namespace Ecommerce.Api.Assistant;

public interface IOrdersAssistantSubAgent
{
    Task<AssistantQueryResponse> HandleAsync(
        AssistantIntent intent,
        Guid buyerId,
        CancellationToken cancellationToken);
}

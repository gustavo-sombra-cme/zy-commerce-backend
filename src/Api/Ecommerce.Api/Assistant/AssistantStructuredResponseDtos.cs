namespace Ecommerce.Api.Assistant;

public static class AssistantResponseTypes
{
    public const string RecentOrders = "recentOrders";
    public const string OrderSummaryAnalytics = "orderSummaryAnalytics";
    public const string OrderedProducts = "orderedProducts";
    public const string MatchingOrders = "matchingOrders";
    public const string ProductFrequency = "productFrequency";
    public const string CatalogProducts = "catalogProducts";
    public const string CatalogProduct = "catalogProduct";
}

public sealed record AssistantOrdersData(
    IReadOnlyCollection<AssistantOrderCardDto> Orders);

public sealed record AssistantMatchingOrdersData(
    IReadOnlyCollection<AssistantOrderCardDto> Orders,
    AssistantCriteriaDto Criteria);

public sealed record AssistantOrderSummaryAnalyticsData(
    decimal TotalSpend,
    int OrderCount);

public sealed record AssistantOrderedProductsData(
    IReadOnlyCollection<AssistantOrderedProductDto> Products);

public sealed record AssistantProductFrequencyData(
    AssistantOrderedProductDto? Product);

public sealed record AssistantCatalogProductsData(
    IReadOnlyCollection<AssistantProductCardDto> Products,
    decimal? MaxPrice);

public sealed record AssistantCatalogProductData(
    AssistantProductCardDto Product);

public sealed record AssistantOrderCardDto(
    Guid OrderId,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    int LineCount,
    IReadOnlyCollection<AssistantOrderLineDto> Lines);

public sealed record AssistantOrderLineDto(
    Guid ProductId,
    string ProductSku,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record AssistantOrderedProductDto(
    string ProductSku,
    string ProductName,
    int Quantity);

public sealed record AssistantProductCardDto(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    bool IsActive);

public sealed record AssistantCriteriaDto(
    decimal? Amount,
    string? SearchText,
    Guid? ProductId);

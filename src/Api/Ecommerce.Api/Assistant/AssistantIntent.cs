namespace Ecommerce.Api.Assistant;

public enum AssistantIntentKind
{
    Unsupported,
    RecentOrders,
    TotalSpend,
    ProductsOrdered,
    OrdersContainingProduct,
    OrdersAboveAmount,
    OrdersContainingProductsOverAmount,
    ProductFrequency,
    CatalogSearchProducts,
    CatalogGetProductBySearch,
    CatalogProductsUnderPrice,
    CatalogGetProduct
}

public sealed record AssistantIntent(
    AssistantIntentKind Kind,
    string? SearchText = null,
    decimal? Amount = null,
    Guid? ProductId = null);

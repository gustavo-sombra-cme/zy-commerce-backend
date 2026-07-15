namespace Ecommerce.Api.Assistant;

internal static class AssistantIntentToolPlan
{
    public static IReadOnlyCollection<string> GetExpectedTools(AssistantIntentKind kind) =>
        kind switch
        {
            AssistantIntentKind.RecentOrders => [AssistantToolNames.OrdersSearch],
            AssistantIntentKind.TotalSpend => [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze],
            AssistantIntentKind.ProductsOrdered => [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantIntentKind.OrdersContainingProduct => [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantIntentKind.OrdersAboveAmount => [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze],
            AssistantIntentKind.OrdersContainingProductsOverAmount => [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantIntentKind.ProductFrequency => [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantIntentKind.CatalogSearchProducts => [AssistantToolNames.CatalogSearch],
            AssistantIntentKind.CatalogProductsUnderPrice => [AssistantToolNames.CatalogSearch],
            AssistantIntentKind.CatalogGetProduct => [AssistantToolNames.CatalogGetProduct],
            _ => Array.Empty<string>()
        };
}

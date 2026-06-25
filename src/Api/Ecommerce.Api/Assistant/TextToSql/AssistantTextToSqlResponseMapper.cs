using System.Globalization;

namespace Ecommerce.Api.Assistant.TextToSql;

public sealed class AssistantTextToSqlResponseMapper
{
    public AssistantQueryResponse? Map(
        AssistantTextToSqlPlan plan,
        AssistantSqlResult result)
    {
        if (!plan.Supported || !result.Succeeded)
        {
            return null;
        }

        return plan.ResultShape switch
        {
            AssistantTextToSqlResultShape.OrderList => MapOrderList(result),
            AssistantTextToSqlResultShape.SpendSummary => MapSpendSummary(result),
            AssistantTextToSqlResultShape.ProductList => MapProductList(result),
            AssistantTextToSqlResultShape.ProductDetails => MapProductDetails(result),
            AssistantTextToSqlResultShape.OrderDetails => MapOrderDetails(result),
            _ => null
        };
    }

    private static AssistantQueryResponse MapOrderList(AssistantSqlResult result)
    {
        var orders = result.Rows
            .Select(TryMapOrderCard)
            .Where(order => order is not null)
            .Select(order => order!)
            .ToArray();

        if (orders.Length == 0)
        {
            return new AssistantQueryResponse(
                "You do not have any recent orders.",
                [AssistantToolNames.OrdersSearch],
                "authenticated-user",
                false,
                AssistantResponseTypes.RecentOrders,
                new AssistantOrdersData(Array.Empty<AssistantOrderCardDto>()));
        }

        var lines = orders.Select(order =>
            $"{order.OrderId} ({order.Status}) total {Money(order.TotalAmount)} on {order.CreatedAt:yyyy-MM-dd}");

        return new AssistantQueryResponse(
            "Your recent orders are: " + string.Join("; ", lines) + ".",
            [AssistantToolNames.OrdersSearch],
            "authenticated-user",
            false,
            AssistantResponseTypes.RecentOrders,
            new AssistantOrdersData(orders));
    }

    private static AssistantQueryResponse? MapSpendSummary(AssistantSqlResult result)
    {
        if (result.Rows.Count == 0)
        {
            return new AssistantQueryResponse(
                "Your total spend across the returned 0 order(s) is 0.00.",
                [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze],
                "authenticated-user",
                false,
                AssistantResponseTypes.OrderSummaryAnalytics,
                new AssistantOrderSummaryAnalyticsData(0m, 0));
        }

        var row = result.Rows[0];
        if (!TryGetDecimal(row, "TotalSpend", out var totalSpend)
            || !TryGetInt(row, "TotalOrders", out var orderCount))
        {
            return null;
        }

        return new AssistantQueryResponse(
            $"Your total spend across the returned {orderCount} order(s) is {Money(totalSpend)}.",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze],
            "authenticated-user",
            false,
            AssistantResponseTypes.OrderSummaryAnalytics,
            new AssistantOrderSummaryAnalyticsData(totalSpend, orderCount));
    }

    private static AssistantQueryResponse MapProductList(AssistantSqlResult result)
    {
        var products = result.Rows
            .Select(TryMapProductCard)
            .Where(product => product is not null)
            .Select(product => product!)
            .ToArray();

        if (products.Length == 0)
        {
            return new AssistantQueryResponse(
                "I did not find matching products in the approved catalog view.",
                [AssistantToolNames.CatalogSearch],
                "catalog-public",
                false,
                AssistantResponseTypes.CatalogProducts,
                new AssistantCatalogProductsData(Array.Empty<AssistantProductCardDto>(), null));
        }

        var lines = products.Select(product =>
            $"{product.Name} ({product.Sku}) {Money(product.Price)}");

        return new AssistantQueryResponse(
            "Matching products: " + string.Join("; ", lines) + ".",
            [AssistantToolNames.CatalogSearch],
            "catalog-public",
            false,
            AssistantResponseTypes.CatalogProducts,
            new AssistantCatalogProductsData(products, null));
    }

    private static AssistantQueryResponse? MapProductDetails(AssistantSqlResult result)
    {
        if (result.Rows.Count != 1)
        {
            return null;
        }

        var product = TryMapProductCard(result.Rows[0]);
        if (product is null)
        {
            return null;
        }

        return new AssistantQueryResponse(
            $"{product.Name} ({product.Sku}) costs {Money(product.Price)} and is {(product.IsActive ? "active" : "inactive")}.",
            [AssistantToolNames.CatalogGetProduct],
            "catalog-public",
            false,
            AssistantResponseTypes.CatalogProduct,
            new AssistantCatalogProductData(product));
    }

    private static AssistantQueryResponse? MapOrderDetails(AssistantSqlResult result)
    {
        var lines = result.Rows
            .Select(TryMapOrderLine)
            .Where(line => line is not null)
            .Select(line => line!)
            .ToArray();

        if (lines.Length == 0)
        {
            return null;
        }

        var lineText = lines.Select(line =>
            $"{line.ProductName} ({line.ProductSku}) x{line.Quantity} at {Money(line.UnitPrice)}");

        return new AssistantQueryResponse(
            "Order line details: " + string.Join("; ", lineText) + ".",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder],
            "authenticated-user",
            false,
            AssistantResponseTypes.OrderedProducts,
            new AssistantOrderedProductsData(lines
                .Select(line => new AssistantOrderedProductDto(line.ProductSku, line.ProductName, line.Quantity))
                .ToArray()));
    }

    private static AssistantOrderCardDto? TryMapOrderCard(AssistantSqlRow row)
    {
        if (!TryGetGuid(row, "OrderId", out var orderId)
            || !TryGetString(row, "Status", out var status)
            || !TryGetDecimal(row, "TotalAmount", out var totalAmount)
            || !TryGetDateTimeOffset(row, "CreatedAt", out var createdAt))
        {
            return null;
        }

        _ = TryGetInt(row, "LineCount", out var lineCount);

        return new AssistantOrderCardDto(
            orderId,
            status,
            totalAmount,
            createdAt,
            lineCount,
            Array.Empty<AssistantOrderLineDto>());
    }

    private static AssistantProductCardDto? TryMapProductCard(AssistantSqlRow row)
    {
        if (!TryGetGuid(row, "ProductId", out var productId)
            || !TryGetString(row, "Sku", out var sku)
            || !TryGetString(row, "Name", out var name)
            || !TryGetDecimal(row, "PriceAmount", out var price)
            || !TryGetBool(row, "IsActive", out var isActive))
        {
            return null;
        }

        _ = TryGetString(row, "Description", out var description);

        return new AssistantProductCardDto(
            productId,
            sku,
            name,
            description,
            price,
            isActive);
    }

    private static AssistantOrderLineDto? TryMapOrderLine(AssistantSqlRow row)
    {
        if (!TryGetGuid(row, "ProductId", out var productId)
            || !TryGetString(row, "ProductSku", out var sku)
            || !TryGetString(row, "ProductName", out var name)
            || !TryGetInt(row, "Quantity", out var quantity)
            || !TryGetDecimal(row, "UnitPriceAmount", out var unitPrice)
            || !TryGetDecimal(row, "LineTotal", out var lineTotal))
        {
            return null;
        }

        return new AssistantOrderLineDto(
            productId,
            sku,
            name,
            unitPrice,
            quantity,
            lineTotal);
    }

    private static bool TryGetString(AssistantSqlRow row, string column, out string value)
    {
        if (row.Values.TryGetValue(column, out var raw) && raw is not null)
        {
            value = Convert.ToString(raw, CultureInfo.InvariantCulture) ?? string.Empty;
            return !string.IsNullOrWhiteSpace(value);
        }

        value = string.Empty;
        return false;
    }

    private static bool TryGetGuid(AssistantSqlRow row, string column, out Guid value)
    {
        if (row.Values.TryGetValue(column, out var raw))
        {
            if (raw is Guid guid)
            {
                value = guid;
                return true;
            }

            if (raw is string text && Guid.TryParse(text, out guid))
            {
                value = guid;
                return true;
            }
        }

        value = Guid.Empty;
        return false;
    }

    private static bool TryGetDecimal(AssistantSqlRow row, string column, out decimal value)
    {
        if (row.Values.TryGetValue(column, out var raw) && raw is not null)
        {
            try
            {
                value = Convert.ToDecimal(raw, CultureInfo.InvariantCulture);
                return true;
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
        }

        value = 0m;
        return false;
    }

    private static bool TryGetInt(AssistantSqlRow row, string column, out int value)
    {
        if (row.Values.TryGetValue(column, out var raw) && raw is not null)
        {
            try
            {
                value = Convert.ToInt32(raw, CultureInfo.InvariantCulture);
                return true;
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
        }

        value = 0;
        return false;
    }

    private static bool TryGetBool(AssistantSqlRow row, string column, out bool value)
    {
        if (row.Values.TryGetValue(column, out var raw) && raw is not null)
        {
            try
            {
                value = Convert.ToBoolean(raw, CultureInfo.InvariantCulture);
                return true;
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
        }

        value = false;
        return false;
    }

    private static bool TryGetDateTimeOffset(AssistantSqlRow row, string column, out DateTimeOffset value)
    {
        if (row.Values.TryGetValue(column, out var raw))
        {
            switch (raw)
            {
                case DateTimeOffset dateTimeOffset:
                    value = dateTimeOffset;
                    return true;
                case DateTime dateTime:
                    value = new DateTimeOffset(dateTime);
                    return true;
                case string text when DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed):
                    value = parsed;
                    return true;
            }
        }

        value = default;
        return false;
    }

    private static string Money(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);
}

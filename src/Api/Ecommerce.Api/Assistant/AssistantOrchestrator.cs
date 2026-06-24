using System.Globalization;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Api.Assistant;

public sealed class AssistantOrchestrator(
    ISender sender,
    IAssistantIntentInterpreter intentInterpreter,
    DeterministicAssistantIntentInterpreter deterministicIntentInterpreter,
    AssistantIntentPlanValidator intentPlanValidator,
    AssistantToolRegistry toolRegistry,
    ILogger<AssistantOrchestrator> logger,
    IOptions<AssistantLlmOptions> llmOptions)
{
    private const int AnalysisPageNumber = 1;
    private const int AnalysisPageSize = 100;

    public async Task<AssistantQueryResponse> QueryAsync(
        string question,
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        var intent = await InterpretIntentAsync(question, cancellationToken);

        return intent.Kind switch
        {
            AssistantIntentKind.RecentOrders => await RecentOrdersAsync(buyerId, cancellationToken),
            AssistantIntentKind.TotalSpend => await TotalSpendAsync(buyerId, cancellationToken),
            AssistantIntentKind.ProductsOrdered => await ProductsOrderedAsync(buyerId, cancellationToken),
            AssistantIntentKind.OrdersContainingProduct => await OrdersContainingProductAsync(intent, buyerId, cancellationToken),
            AssistantIntentKind.OrdersAboveAmount => await OrdersAboveAmountAsync(intent.Amount ?? 0, buyerId, cancellationToken),
            AssistantIntentKind.OrdersContainingProductsOverAmount => await OrdersContainingProductsOverAmountAsync(intent.Amount ?? 0, buyerId, cancellationToken),
            AssistantIntentKind.ProductFrequency => await ProductFrequencyAsync(buyerId, cancellationToken),
            AssistantIntentKind.CatalogProductsUnderPrice => await CatalogProductsUnderPriceAsync(intent.Amount ?? 0, cancellationToken),
            AssistantIntentKind.CatalogGetProduct => await CatalogGetProductAsync(intent.ProductId, cancellationToken),
            _ => Unsupported()
        };
    }

    private async Task<AssistantIntent> InterpretIntentAsync(
        string question,
        CancellationToken cancellationToken)
    {
        LogLlmConfigurationDiagnostics();

        var plan = await TryInterpretAsync(intentInterpreter, question, cancellationToken);
        var deterministicFallbackUsed = false;

        if (plan is null && !ReferenceEquals(intentInterpreter, deterministicIntentInterpreter))
        {
            deterministicFallbackUsed = true;
            plan = await TryInterpretAsync(deterministicIntentInterpreter, question, cancellationToken);
        }

        var validationResult = intentPlanValidator.ValidateWithDiagnostics(question, plan);

        logger.LogInformation(
            "Assistant intent diagnostics: deterministicFallbackUsed={DeterministicFallbackUsed}, modelOutputFailedValidation={ModelOutputFailedValidation}.",
            deterministicFallbackUsed,
            validationResult.ModelOutputFailedValidation);

        return validationResult.Intent;
    }

    private void LogLlmConfigurationDiagnostics()
    {
        var options = llmOptions.Value;

        logger.LogInformation(
            "Assistant LLM configuration diagnostics: llmEnabled={LlmEnabled}, providerEndpointPresent={ProviderEndpointPresent}, providerEndpointValid={ProviderEndpointValid}, modelPresent={ModelPresent}, apiKeyEnvironmentVariableNamePresent={ApiKeyEnvironmentVariableNamePresent}, apiKeyResolved={ApiKeyResolved}.",
            options.Enabled,
            !string.IsNullOrWhiteSpace(options.ResolvedEndpoint),
            IsHttpsEndpoint(options.ResolvedEndpoint),
            !string.IsNullOrWhiteSpace(options.ResolvedModel),
            !string.IsNullOrWhiteSpace(options.ResolvedApiKeyEnvironmentVariable),
            IsApiKeyResolved(options));

        if (!options.Enabled)
        {
            logger.LogInformation(
                "Assistant LLM provider diagnostics: providerCallAttempted={ProviderCallAttempted}, providerCallFailed={ProviderCallFailed}.",
                false,
                false);
        }
    }

    private static async Task<AssistantIntentPlan?> TryInterpretAsync(
        IAssistantIntentInterpreter interpreter,
        string question,
        CancellationToken cancellationToken)
    {
        try
        {
            return await interpreter.InterpretAsync(question, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsApiKeyResolved(AssistantLlmOptions options)
    {
        return options.TryResolveApiKey(out _);
    }

    private static bool IsHttpsEndpoint(string endpoint) =>
        Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
        && uri.Scheme == Uri.UriSchemeHttps;

    public AssistantQueryResponse Unsupported() =>
        new(
            "I can help with read-only product lookup and your own order history, but I cannot perform that request.",
            Array.Empty<string>(),
            "none",
            true);

    private async Task<AssistantQueryResponse> RecentOrdersAsync(Guid buyerId, CancellationToken cancellationToken)
    {
        var orders = await SearchOrdersAsync(buyerId, cancellationToken);
        var items = orders.Items.Take(5).ToArray();

        if (items.Length == 0)
        {
            return Supported("You do not have any recent orders.", [AssistantToolNames.OrdersSearch]);
        }

        var lines = items.Select(order =>
            $"{order.OrderId} ({order.Status}) total {Money(order.TotalAmount)} on {order.CreatedAt:yyyy-MM-dd}");

        return Supported(
            "Your recent orders are: " + string.Join("; ", lines) + ".",
            [AssistantToolNames.OrdersSearch],
            AssistantResponseTypes.RecentOrders,
            new AssistantOrdersData(items.Select(ToOrderCard).ToArray()));
    }

    private async Task<AssistantQueryResponse> TotalSpendAsync(Guid buyerId, CancellationToken cancellationToken)
    {
        var orders = await SearchOrdersAsync(buyerId, cancellationToken);
        var total = orders.Items.Sum(order => order.TotalAmount);

        return Supported(
            $"Your total spend across the returned {orders.Items.Count} order(s) is {Money(total)}.",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze],
            AssistantResponseTypes.OrderSummaryAnalytics,
            new AssistantOrderSummaryAnalyticsData(total, orders.Items.Count));
    }

    private async Task<AssistantQueryResponse> ProductsOrderedAsync(Guid buyerId, CancellationToken cancellationToken)
    {
        var details = await LoadOwnedOrderDetailsAsync(buyerId, cancellationToken);
        var products = details
            .SelectMany(order => order.Lines)
            .GroupBy(line => ProductKey(line.ProductSku, line.ProductName))
            .Select(group => new AssistantOrderedProductDto(
                group.First().ProductSku,
                group.First().ProductName,
                group.Sum(line => line.Quantity)))
            .OrderBy(product => product.ProductName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(product => product.ProductSku, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (products.Length == 0)
        {
            return Supported("I did not find any ordered products in your returned order history.", [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze]);
        }

        var productLines = products.Select(product =>
            $"{product.ProductName} ({product.ProductSku}) x{product.Quantity}");

        return Supported(
            "Products you ordered: " + string.Join("; ", productLines) + ".",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantResponseTypes.OrderedProducts,
            new AssistantOrderedProductsData(products));
    }

    private async Task<AssistantQueryResponse> OrdersContainingProductAsync(
        AssistantIntent intent,
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        var searchText = intent.SearchText ?? string.Empty;
        var details = await LoadOwnedOrderDetailsAsync(buyerId, cancellationToken);
        var matches = details
            .Where(order => order.Lines.Any(line => MatchesProduct(line, searchText, intent.ProductId)))
            .ToArray();

        if (matches.Length == 0)
        {
            return Supported($"I did not find returned orders containing product '{searchText}'.", [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze]);
        }

        return Supported(
            $"I found {matches.Length} returned order(s) containing product '{searchText}': {string.Join(", ", matches.Select(order => order.OrderId))}.",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantResponseTypes.MatchingOrders,
            new AssistantMatchingOrdersData(
                matches.Select(ToOrderCard).ToArray(),
                new AssistantCriteriaDto(null, searchText, intent.ProductId)));
    }

    private async Task<AssistantQueryResponse> OrdersAboveAmountAsync(
        decimal amount,
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        var orders = await SearchOrdersAsync(buyerId, cancellationToken);
        var matches = orders.Items.Where(order => order.TotalAmount > amount).ToArray();

        return Supported(
            $"I found {matches.Length} returned order(s) with totals over {Money(amount)}: {FormatOrderIds(matches.Select(order => order.OrderId))}.",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze],
            AssistantResponseTypes.MatchingOrders,
            new AssistantMatchingOrdersData(
                matches.Select(ToOrderCard).ToArray(),
                new AssistantCriteriaDto(amount, null, null)));
    }

    private async Task<AssistantQueryResponse> OrdersContainingProductsOverAmountAsync(
        decimal amount,
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        var details = await LoadOwnedOrderDetailsAsync(buyerId, cancellationToken);
        var matches = details
            .Where(order => order.Lines.Any(line => line.UnitPrice > amount))
            .ToArray();

        return Supported(
            $"I found {matches.Length} returned order(s) containing products over {Money(amount)}: {FormatOrderIds(matches.Select(order => order.OrderId))}.",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantResponseTypes.MatchingOrders,
            new AssistantMatchingOrdersData(
                matches.Select(ToOrderCard).ToArray(),
                new AssistantCriteriaDto(amount, null, null)));
    }

    private async Task<AssistantQueryResponse> ProductFrequencyAsync(Guid buyerId, CancellationToken cancellationToken)
    {
        var details = await LoadOwnedOrderDetailsAsync(buyerId, cancellationToken);
        var top = details
            .SelectMany(order => order.Lines)
            .GroupBy(line => ProductKey(line.ProductSku, line.ProductName))
            .Select(group => new
            {
                Product = new AssistantOrderedProductDto(
                    group.First().ProductSku,
                    group.First().ProductName,
                    group.Sum(line => line.Quantity)),
                Quantity = group.Sum(line => line.Quantity)
            })
            .OrderByDescending(item => item.Quantity)
            .ThenBy(item => item.Product.ProductName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.Product.ProductSku, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        if (top is null)
        {
            return Supported("I did not find any ordered products in your returned order history.", [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersAnalyze]);
        }

        return Supported(
            $"You bought {top.Product.ProductName} ({top.Product.ProductSku}) most often, with quantity {top.Quantity}.",
            [AssistantToolNames.OrdersSearch, AssistantToolNames.OrdersGetOrder, AssistantToolNames.OrdersAnalyze],
            AssistantResponseTypes.ProductFrequency,
            new AssistantProductFrequencyData(top.Product));
    }

    private async Task<AssistantQueryResponse> CatalogProductsUnderPriceAsync(
        decimal amount,
        CancellationToken cancellationToken)
    {
        var products = await sender.Send(
            new SearchProductsQuery(null, true, AnalysisPageNumber, AnalysisPageSize),
            cancellationToken);
        var matches = products.Items
            .Where(product => product.Price < amount)
            .Take(10)
            .ToArray();
        var matchLines = matches
            .Select(product => $"{product.Name} ({product.Sku}) {Money(product.Price)}")
            .ToArray();

        if (matchLines.Length == 0)
        {
            return new AssistantQueryResponse(
                $"I did not find active products under {Money(amount)} in the returned catalog page.",
                [AssistantToolNames.CatalogSearch],
                "catalog-public",
                false);
        }

        return new AssistantQueryResponse(
            $"Products under {Money(amount)}: {string.Join("; ", matchLines)}.",
            [AssistantToolNames.CatalogSearch],
            "catalog-public",
            false,
            AssistantResponseTypes.CatalogProducts,
            new AssistantCatalogProductsData(matches.Select(ToProductCard).ToArray(), amount));
    }

    private async Task<AssistantQueryResponse> CatalogGetProductAsync(
        Guid? productId,
        CancellationToken cancellationToken)
    {
        if (!productId.HasValue)
        {
            return Unsupported();
        }

        var product = await sender.Send(new GetProductByIdQuery(productId.Value), cancellationToken);

        if (product is null)
        {
            return new AssistantQueryResponse(
                "I could not find that product.",
                [AssistantToolNames.CatalogGetProduct],
                "catalog-public",
                false);
        }

        return new AssistantQueryResponse(
            $"{product.Name} ({product.Sku}) costs {Money(product.Price)} and is {(product.IsActive ? "active" : "inactive")}.",
            [AssistantToolNames.CatalogGetProduct],
            "catalog-public",
            false,
            AssistantResponseTypes.CatalogProduct,
            new AssistantCatalogProductData(ToProductCard(product)));
    }

    private async Task<Ecommerce.Orders.Application.Abstractions.PagedResult<OrderSummaryDto>> SearchOrdersAsync(
        Guid buyerId,
        CancellationToken cancellationToken) =>
        await sender.Send(
            new ListOrdersForBuyerQuery(buyerId, AnalysisPageNumber, AnalysisPageSize),
            cancellationToken);

    private async Task<IReadOnlyCollection<OrderDetailsDto>> LoadOwnedOrderDetailsAsync(
        Guid buyerId,
        CancellationToken cancellationToken)
    {
        var orders = await SearchOrdersAsync(buyerId, cancellationToken);
        var details = new List<OrderDetailsDto>();

        foreach (var order in orders.Items)
        {
            var detail = await sender.Send(new GetOrderByIdQuery(order.OrderId, buyerId), cancellationToken);

            if (detail is not null)
            {
                details.Add(detail);
            }
        }

        return details;
    }

    private AssistantQueryResponse Supported(
        string answer,
        IReadOnlyCollection<string> toolsUsed,
        string? responseType = null,
        object? data = null)
    {
        var safeTools = toolsUsed.Where(toolRegistry.IsAllowed).Distinct(StringComparer.Ordinal).ToArray();
        return new AssistantQueryResponse(answer, safeTools, "authenticated-user", false, responseType, data);
    }

    private static AssistantOrderCardDto ToOrderCard(OrderSummaryDto order) =>
        new(
            order.OrderId,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.LineCount,
            Array.Empty<AssistantOrderLineDto>());

    private static AssistantOrderCardDto ToOrderCard(OrderDetailsDto order) =>
        new(
            order.OrderId,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.Lines.Count,
            order.Lines.Select(ToOrderLine).ToArray());

    private static AssistantOrderLineDto ToOrderLine(OrderLineDetailsDto line) =>
        new(
            line.ProductId,
            line.ProductSku,
            line.ProductName,
            line.UnitPrice,
            line.Quantity,
            line.LineTotal);

    private static AssistantProductCardDto ToProductCard(ProductListItemDto product) =>
        new(
            product.ProductId,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive);

    private static AssistantProductCardDto ToProductCard(ProductDetailsDto product) =>
        new(
            product.ProductId,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive);

    private static bool MatchesProduct(OrderLineDetailsDto line, string searchText, Guid? productId)
    {
        if (productId.HasValue && line.ProductId == productId.Value)
        {
            return true;
        }

        return line.ProductSku.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            || line.ProductName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            || line.ProductId.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    private static string ProductKey(string sku, string name) =>
        $"{sku.Trim().ToUpperInvariant()}|{name.Trim().ToUpperInvariant()}";

    private static string FormatOrderIds(IEnumerable<Guid> orderIds)
    {
        var ids = orderIds.ToArray();
        return ids.Length == 0 ? "none" : string.Join(", ", ids);
    }

    private static string Money(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);
}

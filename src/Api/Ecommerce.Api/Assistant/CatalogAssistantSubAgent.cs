using System.Globalization;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using MediatR;

namespace Ecommerce.Api.Assistant;

public sealed class CatalogAssistantSubAgent(ISender sender) : ICatalogAssistantSubAgent
{
    private const int AnalysisPageNumber = 1;
    private const int AnalysisPageSize = 100;

    public async Task<AssistantQueryResponse> HandleAsync(
        AssistantIntent intent,
        CancellationToken cancellationToken) =>
        intent.Kind switch
        {
            AssistantIntentKind.CatalogSearchProducts => await CatalogSearchProductsAsync(intent.SearchText, cancellationToken),
            AssistantIntentKind.CatalogProductsUnderPrice => await CatalogProductsUnderPriceAsync(intent.Amount ?? 0, cancellationToken),
            AssistantIntentKind.CatalogGetProduct => await CatalogGetProductAsync(intent.ProductId, cancellationToken),
            _ => throw new InvalidOperationException($"Intent kind {intent.Kind} is not handled by the catalog assistant sub-agent.")
        };

    private async Task<AssistantQueryResponse> CatalogSearchProductsAsync(
        string? searchText,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Unsupported();
        }

        var normalizedSearchText = searchText.Trim();
        var products = await sender.Send(
            new SearchProductsQuery(normalizedSearchText, true, AnalysisPageNumber, 10),
            cancellationToken);
        var matches = products.Items.Take(10).ToArray();

        if (matches.Length == 0)
        {
            return new AssistantQueryResponse(
                $"I did not find active products matching \"{normalizedSearchText}\".",
                [AssistantToolNames.CatalogSearch],
                "catalog-public",
                false,
                AssistantResponseTypes.CatalogProducts,
                new AssistantCatalogProductsData(Array.Empty<AssistantProductCardDto>(), null));
        }

        var matchLines = matches
            .Select(product => $"{product.Name} ({product.Sku}) {Money(product.Price)}")
            .ToArray();

        return new AssistantQueryResponse(
            $"Matching active products for \"{normalizedSearchText}\": {string.Join("; ", matchLines)}.",
            [AssistantToolNames.CatalogSearch],
            "catalog-public",
            false,
            AssistantResponseTypes.CatalogProducts,
            new AssistantCatalogProductsData(matches.Select(ToProductCard).ToArray(), null));
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

    private static AssistantQueryResponse Unsupported() =>
        new(
            "I can help with read-only product lookup and your own order history, but I cannot perform that request.",
            Array.Empty<string>(),
            "none",
            true);

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

    private static string Money(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);
}

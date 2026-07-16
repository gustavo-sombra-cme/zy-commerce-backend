using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;

namespace Ecommerce.Api.Assistant;

internal static class AssistantProductMapper
{
    public static AssistantProductCardDto ToCard(ProductListItemDto product) =>
        new(product.ProductId, product.Sku, product.Name, product.Description, product.Price, product.IsActive);

    public static AssistantProductCardDto ToCard(ProductDetailsDto product) =>
        new(product.ProductId, product.Sku, product.Name, product.Description, product.Price, product.IsActive);
}

using System.ComponentModel;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Orders.Application.Orders.CreateOrder;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace Ecommerce.Api.Mcp;

[Authorize]
[McpServerToolType]
public sealed class EcommerceMcpTools(ISender sender)
{
    [McpServerTool(
        Name = "catalog_search_products",
        Title = "Search Catalog Products",
        ReadOnly = true,
        UseStructuredContent = true)]
    [Description("Searches catalog products with optional text, active status, and pagination filters.")]
    public async Task<McpCatalogSearchProductsResult> SearchCatalogProducts(
        string? searchTerm = null,
        bool? isActive = null,
        int? pageNumber = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new SearchProductsQuery(searchTerm, isActive, pageNumber, pageSize),
            cancellationToken);

        return new McpCatalogSearchProductsResult(
            result.Items
                .Select(product => new McpProductListItem(
                    product.ProductId,
                    product.Sku,
                    product.Name,
                    product.Description,
                    product.IsActive,
                    product.CreatedAt))
                .ToArray(),
            result.PageNumber,
            result.PageSize,
            result.TotalCount,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage);
    }

    [McpServerTool(
        Name = "catalog_get_product_by_id",
        Title = "Get Catalog Product By Id",
        ReadOnly = true,
        UseStructuredContent = true)]
    [Description("Gets public catalog product details by product id.")]
    public async Task<McpProductDetails?> GetCatalogProductById(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);

        return product is null
            ? null
            : new McpProductDetails(
                product.ProductId,
                product.Sku,
                product.Name,
                product.Description,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt);
    }

    [McpServerTool(
        Name = "orders_get_order_by_id",
        Title = "Get Order By Id",
        ReadOnly = true,
        UseStructuredContent = true)]
    [Description("Gets an order by id for the authenticated owner only.")]
    public async Task<McpOrderDetails?> GetOrderById(
        RequestContext<CallToolRequestParams> context,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        if (!CurrentUser.TryGetUserId(context.User, out var buyerId))
        {
            throw new UnauthorizedAccessException("An authenticated user is required.");
        }

        var order = await sender.Send(new GetOrderByIdQuery(orderId, buyerId), cancellationToken);

        return order is null ? null : MapOrder(order);
    }

    [McpServerTool(
        Name = "orders_create_order",
        Title = "Create Order",
        Destructive = true,
        OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Creates an order for the authenticated user from supplied product snapshot lines. Requires confirmedByUser to be true.")]
    public async Task<McpCreateOrderResult> CreateOrder(
        RequestContext<CallToolRequestParams> context,
        bool confirmedByUser,
        IReadOnlyCollection<McpCreateOrderLineInput> lines,
        CancellationToken cancellationToken = default)
    {
        if (!confirmedByUser)
        {
            throw new InvalidOperationException("orders_create_order requires confirmedByUser to be true.");
        }

        if (!CurrentUser.TryGetUserId(context.User, out var buyerId))
        {
            throw new UnauthorizedAccessException("An authenticated user is required.");
        }

        var result = await sender.Send(
            new CreateOrderCommand(
                buyerId,
                (lines ?? Array.Empty<McpCreateOrderLineInput>())
                    .Select(line => new CreateOrderLineCommand(
                        line.ProductId,
                        line.ProductSku,
                        line.ProductName,
                        line.UnitPrice,
                        line.Quantity))
                    .ToArray()),
            cancellationToken);

        return new McpCreateOrderResult(
            result.OrderId,
            result.TotalAmount,
            result.CreatedAt);
    }

    private static McpOrderDetails MapOrder(OrderDetailsDto order)
    {
        return new McpOrderDetails(
            order.OrderId,
            order.BuyerId,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.Lines
                .Select(line => new McpOrderLineDetails(
                    line.OrderLineId,
                    line.ProductId,
                    line.ProductSku,
                    line.ProductName,
                    line.UnitPrice,
                    line.Quantity,
                    line.LineTotal))
                .ToArray());
    }
}

public sealed record McpCatalogSearchProductsResult(
    IReadOnlyCollection<McpProductListItem> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record McpProductListItem(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt);

public sealed record McpProductDetails(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed record McpOrderDetails(
    Guid OrderId,
    Guid BuyerId,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<McpOrderLineDetails> Lines);

public sealed record McpOrderLineDetails(
    Guid OrderLineId,
    Guid ProductId,
    string ProductSku,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record McpCreateOrderLineInput(
    Guid ProductId,
    string ProductSku,
    string ProductName,
    decimal UnitPrice,
    int Quantity);

public sealed record McpCreateOrderResult(
    Guid OrderId,
    decimal TotalAmount,
    DateTimeOffset CreatedAt);

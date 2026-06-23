using Ecommerce.Catalog.Application.Products.CreateProduct;
using Ecommerce.Catalog.Application.Products.DeactivateProduct;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.ReactivateProduct;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Catalog.Application.Products.UpdateProductDetails;
using Ecommerce.Catalog.Application.Products.UpdateProductPrice;
using Ecommerce.Catalog.Contracts.Products;
using Ecommerce.Api.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Catalog;

[ApiController]
[Route("api/catalog/products")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SearchProductsResponse>> SearchProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] bool? isActive,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SearchProductsQuery(searchTerm, isActive, pageNumber, pageSize),
            cancellationToken);

        var response = new SearchProductsResponse(
            result.Items
                .Select(product => new ProductListItemResponse(
                    product.ProductId,
                    product.Sku,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.IsActive,
                    product.CreatedAt))
                .ToArray(),
            result.PageNumber,
            result.PageSize,
            result.TotalCount,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage);

        return Ok(response);
    }

    [HttpGet("{productId:guid}")]
    public async Task<ActionResult<GetProductByIdResponse>> GetProductById(
        Guid productId,
        CancellationToken cancellationToken)
    {
        if (productId == Guid.Empty)
        {
            return BadRequest(new { message = "Product id cannot be empty." });
        }

        var product = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(new GetProductByIdResponse(
            product.ProductId,
            product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt));
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    [HttpPost]
    [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CreateProductResponse>> CreateProduct(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateProductCommand(request.Sku, request.Name, request.Description, request.Price),
            cancellationToken);

        var response = new CreateProductResponse(result.ProductId, result.Sku, result.Name);

        return Created($"/api/catalog/products/{response.ProductId}", response);
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    [HttpPut("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateProductDetails(
        Guid productId,
        UpdateProductDetailsRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateProductDetailsCommand(productId, request.Name, request.Description),
            cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    [HttpPut("{productId:guid}/price")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProductPrice(
        Guid productId,
        UpdateProductPriceRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateProductPriceCommand(productId, request.Price), cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateProduct(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivateProductCommand(productId), cancellationToken);

        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    [HttpPost("{productId:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactivateProduct(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new ReactivateProductCommand(productId), cancellationToken);

        return NoContent();
    }
}

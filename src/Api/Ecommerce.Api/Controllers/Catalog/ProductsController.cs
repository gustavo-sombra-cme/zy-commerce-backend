using Ecommerce.Catalog.Application.Products.CreateProduct;
using Ecommerce.Catalog.Application.Products.DeactivateProduct;
using Ecommerce.Catalog.Application.Products.GetProductById;
using Ecommerce.Catalog.Application.Products.SearchProducts;
using Ecommerce.Catalog.Contracts.Products;
using MediatR;
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
            product.IsActive,
            product.CreatedAt,
            product.UpdatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> CreateProduct(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateProductCommand(request.Sku, request.Name, request.Description),
            cancellationToken);

        var response = new CreateProductResponse(result.ProductId, result.Sku, result.Name);

        return Created($"/api/catalog/products/{response.ProductId}", response);
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeactivateProduct(
        Guid productId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivateProductCommand(productId), cancellationToken);

        return NoContent();
    }
}

using Ecommerce.Orders.Application.Orders.CreateOrder;
using Ecommerce.Orders.Application.Orders.GetOrderById;
using Ecommerce.Orders.Application.Orders.ListOrdersForBuyer;
using Ecommerce.Orders.Contracts.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Orders;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ListOrdersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ListOrdersResponse>> ListOrders(
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var buyerId))
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new ListOrdersForBuyerQuery(buyerId, pageNumber, pageSize),
            cancellationToken);

        var response = new ListOrdersResponse(
            result.Items
                .Select(order => new OrderSummaryResponse(
                    order.OrderId,
                    order.Status,
                    order.TotalAmount,
                    order.CreatedAt,
                    order.LineCount))
                .ToArray(),
            result.PageNumber,
            result.PageSize,
            result.TotalCount,
            result.TotalPages,
            result.HasPreviousPage,
            result.HasNextPage);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var buyerId))
        {
            return Unauthorized();
        }

        var result = await sender.Send(
            new CreateOrderCommand(
                buyerId,
                (request.Lines ?? Array.Empty<CreateOrderLineRequest>())
                    .Select(line => new CreateOrderLineCommand(
                        line.ProductId,
                        line.ProductSku,
                        line.ProductName,
                        line.UnitPrice,
                        line.Quantity))
                    .ToArray()),
            cancellationToken);

        var response = new CreateOrderResponse(
            result.OrderId,
            result.TotalAmount,
            result.CreatedAt);

        return Created($"/api/orders/{response.OrderId}", response);
    }

    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(GetOrderByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetOrderByIdResponse>> GetOrderById(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        if (orderId == Guid.Empty)
        {
            return BadRequest(new { message = "Order id cannot be empty." });
        }

        if (!TryGetCurrentUserId(out var buyerId))
        {
            return Unauthorized();
        }

        var order = await sender.Send(new GetOrderByIdQuery(orderId, buyerId), cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(new GetOrderByIdResponse(
            order.OrderId,
            order.BuyerId,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.Lines
                .Select(line => new OrderLineResponse(
                    line.OrderLineId,
                    line.ProductId,
                    line.ProductSku,
                    line.ProductName,
                    line.UnitPrice,
                    line.Quantity,
                    line.LineTotal))
                .ToArray()));
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var subject = User.FindFirst("sub")?.Value;
        return Guid.TryParse(subject, out userId);
    }
}

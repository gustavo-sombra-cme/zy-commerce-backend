using Ecommerce.Api.Assistant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers.Assistant;

[ApiController]
[Authorize]
[Route("api/assistant")]
public sealed class AssistantController(AssistantOrchestrator orchestrator) : ControllerBase
{
    [HttpPost("query")]
    [ProducesResponseType(typeof(AssistantQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AssistantQueryResponse>> Query(
        AssistantQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new { message = "Question is required." });
        }

        if (request.Question.Length > 1000)
        {
            return BadRequest(new { message = "Question cannot exceed 1000 characters." });
        }

        if (!TryGetCurrentUserId(out var buyerId))
        {
            return Unauthorized();
        }

        var response = await orchestrator.QueryAsync(
            request.Question.Trim(),
            buyerId,
            cancellationToken);

        return Ok(response);
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var subject = User.FindFirst("sub")?.Value;
        return Guid.TryParse(subject, out userId);
    }
}

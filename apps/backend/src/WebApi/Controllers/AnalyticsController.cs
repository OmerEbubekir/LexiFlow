using LexiFlow.Application.Features.Analytics.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LexiFlow.WebApi.Controllers;

[Authorize]
public class AnalyticsController : BaseApiController
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("categories-report")]
    public async Task<IActionResult> GetCategoryReport()
    {
        var result = await _mediator.Send(new GetAnalyticsReportQuery(GetUserId()));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

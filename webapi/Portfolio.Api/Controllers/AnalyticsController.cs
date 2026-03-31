using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Application.Features.Analytics.Queries;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly ISender _mediator;

    public AnalyticsController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET /api/analytics/dashboard?days=30
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] int days = 30)
    {
        var data = await _mediator.Send(new GetAnalyticsDashboardQuery(days));
        return Ok(data);
    }
}

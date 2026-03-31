using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Application.Features.Settings.Commands;
using PortfolioApi.Application.Features.Settings.Queries;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ISender _mediator;

    public SettingsController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET /api/settings
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var settings = await _mediator.Send(new GetSystemSettingsQuery(category));
        return Ok(settings);
    }

    // GET /api/settings/{key}
    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var setting = await _mediator.Send(new GetSystemSettingByKeyQuery(key));
        return setting != null ? Ok(setting) : NotFound();
    }

    // PUT /api/settings
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSettingRequest request)
    {
        var username = User.Identity?.Name ?? "system";
        var result = await _mediator.Send(new UpdateSystemSettingCommand(
            request.Key, request.Value, request.DataType, username));
        return result ? Ok() : BadRequest();
    }
}

public class UpdateSettingRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
}

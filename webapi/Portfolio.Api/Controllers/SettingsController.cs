using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Application.DTOs;
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
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] string? category)
    {
        var settings = await _mediator.Send(new GetSystemSettingsQuery(category));
        return Ok(new ApiResponse<object> { Success = true, Data = settings });
    }

    // GET /api/settings/{key}
    [HttpGet("{key}")]
    public async Task<ActionResult<ApiResponse<object>>> GetByKey(string key)
    {
        var setting = await _mediator.Send(new GetSystemSettingByKeyQuery(key));
        return setting != null 
            ? Ok(new ApiResponse<object> { Success = true, Data = setting }) 
            : NotFound(new ApiResponse { Success = false, Message = "Setting not found" });
    }

    // PUT /api/settings
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> Update([FromBody] UpdateSettingRequest request)
    {
        var username = User.Identity?.Name ?? "system";
        var result = await _mediator.Send(new UpdateSystemSettingCommand(
            request.Key, request.Value, request.DataType, username));
        return result 
            ? Ok(new ApiResponse { Success = true, Message = "Setting updated successfully" }) 
            : BadRequest(new ApiResponse { Success = false, Message = "Failed to update setting" });
    }
}

public class UpdateSettingRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
}

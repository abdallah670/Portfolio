using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Features.Authentication.Commands;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;
    
    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _mediator.Send(new LoginCommand(request));
        
        if (!response.Success)
            return Unauthorized(response);
        
        return Ok(response);
    }
    
    // PUT /api/auth/password
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var username = User.Identity?.Name ?? string.Empty;
        var result = await _mediator.Send(new UpdatePasswordCommand(
            request.CurrentPassword, request.NewPassword, username));
        
        if (!result.Success)
            return BadRequest(new { errors = result.Errors });
        
        return Ok(new { message = "Password updated successfully" });
    }

    // PUT /api/auth/username
    [HttpPut("username")]
    [Authorize]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequest request)
    {
        var username = User.Identity?.Name ?? string.Empty;
        var result = await _mediator.Send(new UpdateUsernameCommand(
            request.NewUsername, username));
        
        if (!result.Success)
            return BadRequest(new { errors = result.Errors });
        
        return Ok(new { message = "Username updated successfully" });
    }
    

}


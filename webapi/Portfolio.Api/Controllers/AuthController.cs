using FluentValidation;
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
    private readonly IValidator<ChangeUsernameRequest> _usernameValidator;
    private readonly IValidator<ChangePasswordRequest> _passwordValidator;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(ISender mediator, IValidator<ChangeUsernameRequest> usernameValidator, IValidator<ChangePasswordRequest> passwordValidator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _usernameValidator = usernameValidator;
        _passwordValidator = passwordValidator;
        _logger = logger;
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
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        // Manual validation
        var validation = await _passwordValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ApiResponse { Success = false, Message = errors });
        }
        
        var username = User.Identity?.Name ?? string.Empty;
        var result = await _mediator.Send(new UpdatePasswordCommand(
            request.CurrentPassword, request.NewPassword, username));
        
        if (!result.Success)
            return BadRequest(new ApiResponse { Success = false, Message = string.Join(", ", result.Errors) });
        
        return Ok(new ApiResponse { Success = true, Message = "Password updated successfully" });
    }

    // PUT /api/auth/username
    [HttpPut("username")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> ChangeUsername([FromBody] ChangeUsernameRequest request)
    {
        _logger.LogInformation("ChangeUsername called with NewUsername: {NewUsername}, CurrentUser: {CurrentUser}", 
            request.NewUsername, User.Identity?.Name);
        
        // Manual validation
        var validation = await _usernameValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed: {Errors}", errors);
            return BadRequest(new ApiResponse { Success = false, Message = errors });
        }
        
        var username = User.Identity?.Name ?? string.Empty;
        var result = await _mediator.Send(new UpdateUsernameCommand(
            request.NewUsername, username));
        
        if (!result.Success)
        {
            _logger.LogWarning("Username change failed: {Errors}", string.Join(", ", result.Errors));
            return BadRequest(new ApiResponse { Success = false, Message = string.Join(", ", result.Errors) });
        }
        
        return Ok(new ApiResponse { Success = true, Message = "Username updated successfully" });
    }
    

}


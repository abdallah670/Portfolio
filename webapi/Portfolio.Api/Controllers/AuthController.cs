using MediatR;
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
}
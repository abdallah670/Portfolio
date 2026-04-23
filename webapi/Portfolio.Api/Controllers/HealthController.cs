using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(AppDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check database connectivity
            await _context.Database.CanConnectAsync();
            
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                database = "connected"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                status = "unhealthy",
                timestamp = DateTime.UtcNow,
                database = "disconnected",
                error = ex.Message
            });
        }
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { message = "pong", timestamp = DateTime.UtcNow });
    }
}

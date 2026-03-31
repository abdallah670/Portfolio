using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Application.Features.Messaging.Commands;
using PortfolioApi.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly AppDbContext _context;

    public MessagesController(ISender mediator, AppDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    // GET: api/messages
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<object>> GetMessages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRead = null)
    {
        var query = _context.Messages.AsQueryable();

        if (isRead.HasValue)
        {
            query = query.Where(m => m.IsRead == isRead.Value);
        }

        var totalCount = await query.CountAsync();
        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new
            {
                m.Id,
                m.Name,
                m.Email,
                m.Subject,
                Preview = m.Content.Length > 100 ? m.Content.Substring(0, 100) + "..." : m.Content,
                m.IsRead,
                m.CreatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            items = messages,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    // GET: api/messages/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var message = await _context.Messages.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        return message;
    }

    // POST: api/messages (Public - Contact Form)
    [HttpPost]
    public async Task<ActionResult<Message>> CreateMessage([FromBody] CreateMessageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var command = new CreateMessageCommand(
            request.Name,
            request.Email,
            request.Subject,
            request.Content,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers["User-Agent"].ToString()
        );

        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    // PUT: api/messages/5/read
    [HttpPut("{id}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var message = await _context.Messages.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/messages/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var message = await _context.Messages.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/messages/unread-count
    [HttpGet("unread-count")]
    [Authorize]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var count = await _context.Messages.CountAsync(m => !m.IsRead);
        return Ok(count);
    }
    //post api/messages/{id}/respond
    [HttpPost("{id}/respond")]
    [Authorize]
    public async Task<IActionResult> RespondToMessage(int id, [FromBody] RespondToMessageRequest request)
    {
        var message = await _context.Messages.FindAsync(id);

        if (message == null)
        {
            return NotFound();
        }

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}



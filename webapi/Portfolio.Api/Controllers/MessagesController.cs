using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Application.Features.Messaging.Commands;
using PortfolioApi.Application.Features.Messaging.Queries;
using System.ComponentModel.DataAnnotations;
using PortfolioApi.Api.Models;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(ISender mediator, ILogger<MessagesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // GET: api/messages
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetMessages(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRead = null)
    {
        _logger.LogInformation("API: Getting messages page {Page}", page);
        var result = await _mediator.Send(new GetMessagesQuery { Page = page, PageSize = pageSize, IsRead = isRead });
        return Ok(new ApiResponse<object> { Success = true, Data = result });
    }

    // GET: api/messages/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<Message>>> GetMessage(int id)
    {
        _logger.LogInformation("API: Getting message {MessageId}", id);
        var message = await _mediator.Send(new GetMessageByIdQuery { Id = id });

        if (message == null)
        {
            _logger.LogWarning("API: Message {MessageId} not found", id);
            return NotFound(new ApiResponse { Success = false, Message = "Message not found" });
        }

        return Ok(new ApiResponse<Message> { Success = true, Data = message });
    }

    // POST: api/messages (Public - Contact Form)
    [HttpPost]
    public async Task<ActionResult<Message>> CreateMessage([FromBody] CreateMessageRequest request)
    {
        _logger.LogInformation("API: Creating message from {Email}", request.Email);
        
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
        {
            _logger.LogWarning("API: Failed to create message from {Email}", request.Email);
            return BadRequest(result);
        }

        _logger.LogInformation("API: Message created successfully");
        return Ok(result);
    }

    // PUT: api/messages/5/read
    [HttpPut("{id}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        _logger.LogInformation("API: Marking message {MessageId} as read", id);
        var result = await _mediator.Send(new MarkMessageAsReadCommand { Id = id });
        return result ? NoContent() : NotFound();
    }

    // PUT: api/messages/read-all
    [HttpPut("read-all")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<int>>> MarkAllAsRead()
    {
        _logger.LogInformation("API: Marking all messages as read");
        var count = await _mediator.Send(new MarkAllMessagesAsReadCommand());
        return Ok(new ApiResponse<int> { Success = true, Message = $"{count} messages marked as read", Data = count });
    }

    // DELETE: api/messages/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        _logger.LogInformation("API: Deleting message {MessageId}", id);
        var result = await _mediator.Send(new DeleteMessageCommand { Id = id });
        return result ? NoContent() : NotFound();
    }

    // GET: api/messages/unread-count
    [HttpGet("unread-count")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        _logger.LogDebug("API: Getting unread message count");
        var count = await _mediator.Send(new GetUnreadMessageCountQuery());
        return Ok(new ApiResponse<int> { Success = true, Data = count });
    }

    // POST api/messages/{id}/respond
    [HttpPost("{id}/respond")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> RespondToMessage(int id, [FromBody] RespondToMessageRequest request)
    {
        _logger.LogInformation("API: Responding to message {MessageId}", id);
        
        var result = await _mediator.Send(new RespondToMessageCommand 
        { 
            MessageId = id, 
            ResponseContent = request.Content 
        });

        if (!result.Success)
        {
            _logger.LogError("API: Failed to respond to message {MessageId}: {Error}", id, result.Error);
            return StatusCode(500, new ApiResponse { Success = false, Message = result.Error });
        }

        _logger.LogInformation("API: Response to message {MessageId} sent successfully", id);
        return Ok(new ApiResponse { Success = true, Message = "Response sent successfully" });
    }
}



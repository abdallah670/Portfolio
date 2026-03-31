using System;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class Message {
    [Key] public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsReplied { get; set; }
    public DateTime? RepliedAt { get; set; }
}

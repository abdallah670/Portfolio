namespace PortfolioApi.Api.Models
public class CreateMessageRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Subject { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;
}
public class RespondToMessageRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
}

namespace PortfolioApi.Domain.Entities;
public class Project{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Stack { get; set; } = string.Empty; // JSON serialized list
    public string Image { get; set; } = string.Empty;
    public string linkedinUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public string LiveUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPublished { get; set; } = true;
    public int ViewsCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

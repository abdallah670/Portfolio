namespace PortfolioApi.Domain.Entities;
public class Project{
     public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Stack { get; set; } = new();
    public string Image { get; set; } = string.Empty;
    public string LiveUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
}
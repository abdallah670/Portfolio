using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class AboutCard {
    [Key] public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

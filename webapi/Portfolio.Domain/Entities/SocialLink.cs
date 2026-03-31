using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class SocialLink {
    [Key] public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

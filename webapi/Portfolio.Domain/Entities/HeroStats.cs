using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class HeroStats {
    [Key] public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

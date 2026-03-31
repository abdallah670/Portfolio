using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class Achievement {
    [Key] public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

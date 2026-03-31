using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class JourneyItem {
    [Key] public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Org { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

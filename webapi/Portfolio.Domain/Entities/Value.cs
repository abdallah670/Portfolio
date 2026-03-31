using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class Value {
    [Key] public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

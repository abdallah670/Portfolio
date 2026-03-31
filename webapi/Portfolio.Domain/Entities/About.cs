using System;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class About 
{
    [Key] 
    public int Id { get; set; }
    public string Kicker { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string FunFact { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

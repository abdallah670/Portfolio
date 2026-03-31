using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class Skill {
    [Key] public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public virtual SkillCategory? Category { get; set; }
}

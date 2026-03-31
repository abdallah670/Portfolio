using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class SkillCategory {
    [Key] public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}

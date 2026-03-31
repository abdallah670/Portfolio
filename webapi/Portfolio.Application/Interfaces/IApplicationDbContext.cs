using Microsoft.EntityFrameworkCore;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Hero> Heroes { get; set; }
    DbSet<HeroStats> HeroStats { get; set; }
    DbSet<About> Abouts { get; set; }
    DbSet<AboutCard> AboutCards { get; set; }
    DbSet<Achievement> Achievements { get; set; }
    DbSet<Value> Values { get; set; }
    DbSet<SkillCategory> SkillCategories { get; set; }
    DbSet<Skill> Skills { get; set; }
    DbSet<Project> Projects { get; set; }
    DbSet<JourneyItem> JourneyItems { get; set; }
    DbSet<SocialLink> SocialLinks { get; set; }
    DbSet<Contact> Contacts { get; set; }
    DbSet<Message> Messages { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

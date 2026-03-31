using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;

namespace PortfolioApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<AdminUser> AdminUsers { get; set; }
    public DbSet<Hero> Heroes { get; set; }
    public DbSet<HeroStats> HeroStats { get; set; }
    public DbSet<About> Abouts { get; set; }
    public DbSet<AboutCard> AboutCards { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<Value> Values { get; set; }
    public DbSet<SkillCategory> SkillCategories { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<JourneyItem> JourneyItems { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Message> Messages { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Skills)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
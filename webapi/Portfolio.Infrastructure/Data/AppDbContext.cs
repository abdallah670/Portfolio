using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AdminUser, IdentityRole<int>, int>, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Hero> Heroes { get; set; }
    public DbSet<HeroStats> HeroStats { get; set; }
    public DbSet<SkillCategory> SkillCategories { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<JourneyItem> JourneyItems { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Skill>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Skills)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SystemSetting>()
            .HasIndex(s => s.Key)
            .IsUnique();
    }
}
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class PortfolioService
{
    private readonly AppDbContext _context;
    
    public PortfolioService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<PortfolioConfigDto> GetFullConfigAsync()
    {
        var hero = await _context.Heroes.FirstOrDefaultAsync() ?? new Hero();
        var heroStats = await _context.HeroStats.OrderBy(s => s.DisplayOrder).ToListAsync();
        var about = await _context.Abouts.FirstOrDefaultAsync() ?? new About();
        var aboutCards = await _context.AboutCards.OrderBy(c => c.DisplayOrder).ToListAsync();
        var achievements = await _context.Achievements.OrderBy(a => a.DisplayOrder).ToListAsync();
        var values = await _context.Values.OrderBy(v => v.DisplayOrder).ToListAsync();
        var skillCategories = await _context.SkillCategories.OrderBy(c => c.DisplayOrder).ToListAsync();
        var projects = await _context.Projects.OrderByDescending(p => p.IsFeatured).ThenBy(p => p.DisplayOrder).ToListAsync();
        var journey = await _context.JourneyItems.OrderBy(j => j.DisplayOrder).ToListAsync();
        var socials = await _context.SocialLinks.ToListAsync();
        var contact = await _context.Contacts.FirstOrDefaultAsync() ?? new Contact();
        
        return new PortfolioConfigDto
        {
            Hero = new HeroDto
            {
                Name = hero.Name,
                HeadlineTop = hero.HeadlineTop,
                HeadlineMain = hero.HeadlineMain,
                AvailabilityLabel = hero.AvailabilityLabel,
                Subtitle = hero.Subtitle,
                HeroIntro = hero.HeroIntro,
                CtaPrimaryLabel = hero.CtaPrimaryLabel,
                CtaPrimaryHref = hero.CtaPrimaryHref,
                CtaSecondaryLabel = hero.CtaSecondaryLabel,
                CtaSecondaryHref = hero.CtaSecondaryHref,
                ProfileImage = hero.ProfileImage,
                Stats = heroStats.Select(s => new HeroStatsDto { Label = s.Label, Value = s.Value }).ToList()
            },
            About = new AboutDto
            {
                Kicker = about.Kicker,
                Title = about.Title,
                Subtitle = about.Subtitle,
                FunFact = about.FunFact,
                Cards = aboutCards.Select(c => new AboutCardDto { Title = c.Title, Subtitle = c.Subtitle }).ToList(),
                Achievements = achievements.Select(a => a.Text).ToList(),
                Values = values.Select(v => new ValueDto { Title = v.Title, Description = v.Description }).ToList()
            },
            Skills = await GetSkillCategoriesAsync(),
            FeaturedProjects = projects.Where(p => p.IsFeatured).Select(MapProject).ToList(),
            MoreProjects = projects.Where(p => !p.IsFeatured).Select(MapProject).ToList(),
            Journey = journey.Select(j => new JourneyItemDto
            {
                Id = j.Id,
                Title = j.Title,
                Period = j.Period,
                Org = j.Org,
                Description = j.Description
            }).ToList(),
            Socials = socials.Select(s => new SocialLinkDto { Label = s.Label, Href = s.Href, Icon = s.Icon }).ToList(),
            Contact = new ContactDto
            {
                Email = contact.Email,
                WhatsApp = contact.WhatsApp,
                Phone = contact.Phone,
                Location = contact.Location
            }
        };
    }
    
    public async Task<List<SkillCategoryDto>> GetSkillCategoriesAsync()
    {
        var categories = await _context.SkillCategories
            .Include(c => c.Skills)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        
        return categories.Select(c => new SkillCategoryDto
        {
            Title = c.Title,
            Color = c.Color,
            Skills = c.Skills.Select(s => new SkillDto { Name = s.Name, Level = s.Level }).ToList()
        }).ToList();
    }
    
    public async Task<ApiResponse<Hero>> UpdateHeroAsync(Hero heroDto)
    {
        var hero = await _context.Heroes.FirstOrDefaultAsync();
        if (hero == null)
        {
            hero = new Hero { Id = 0 };
            _context.Heroes.Add(hero);
        }
        
        hero.Name = heroDto.Name;
        hero.HeadlineTop = heroDto.HeadlineTop;
        hero.HeadlineMain = heroDto.HeadlineMain;
        hero.AvailabilityLabel = heroDto.AvailabilityLabel;
        hero.Subtitle = heroDto.Subtitle;
        hero.HeroIntro = heroDto.HeroIntro;
        hero.CtaPrimaryLabel = heroDto.CtaPrimaryLabel;
        hero.CtaPrimaryHref = heroDto.CtaPrimaryHref;
        hero.CtaSecondaryLabel = heroDto.CtaSecondaryLabel;
        hero.CtaSecondaryHref = heroDto.CtaSecondaryHref;
        hero.ProfileImage = heroDto.ProfileImage;
        hero.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return new ApiResponse<Hero> { Success = true, Message = "Hero updated", Data = hero };
    }
    
    public async Task<ApiResponse<About>> UpdateAboutAsync(About aboutDto)
    {
        var about = await _context.Abouts.FirstOrDefaultAsync();
        if (about == null)
        {
            about = new About { Id = 0 };
            _context.Abouts.Add(about);
        }
        
        about.Kicker = aboutDto.Kicker;
        about.Title = aboutDto.Title;
        about.Subtitle = aboutDto.Subtitle;
        about.FunFact = aboutDto.FunFact;
        about.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return new ApiResponse<About> { Success = true, Message = "About updated", Data = about };
    }
    
    public async Task<ApiResponse<Project>> CreateProjectAsync(Project projectDto)
    {
        var project = new Project
        {
            Title = projectDto.Title,
            Year = projectDto.Year,
            Category = projectDto.Category,
            Description = projectDto.Description,
            Stack = projectDto.Stack,
            Image = projectDto.Image,
            LiveUrl = projectDto.LiveUrl,
            GithubUrl = projectDto.GithubUrl,
            Status = projectDto.Status,
            Color = projectDto.Color,
            IsFeatured = projectDto.IsFeatured,
            DisplayOrder = projectDto.DisplayOrder
        };
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<Project> { Success = true, Message = "Project created", Data = project };
    }
    
    public async Task<ApiResponse<Project>> UpdateProjectAsync(Project projectDto)
    {
        var project = await _context.Projects.FindAsync(projectDto.Id);
        if (project == null)
            return new ApiResponse<Project> { Success = false, Message = "Project not found" };
        
        project.Title = projectDto.Title;
        project.Year = projectDto.Year;
        project.Category = projectDto.Category;
        project.Description = projectDto.Description;
        project.Stack = projectDto.Stack;
        project.Image = projectDto.Image;
        project.LiveUrl = projectDto.LiveUrl;
        project.GithubUrl = projectDto.GithubUrl;
        project.Status = projectDto.Status;
        project.Color = projectDto.Color;
        project.IsFeatured = projectDto.IsFeatured;
        project.DisplayOrder = projectDto.DisplayOrder;
        
        await _context.SaveChangesAsync();
        
        return new ApiResponse<Project> { Success = true, Message = "Project updated", Data = project };
    }
    
    public async Task<ApiResponse> DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return new ApiResponse { Success = false, Message = "Project not found" };
        
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        
        return new ApiResponse { Success = true, Message = "Project deleted" };
    }
    
    public async Task<List<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _context.Projects
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync();
        
        return projects.Select(MapProject).ToList();
    }
    
    public async Task<ApiResponse<JourneyItem>> CreateJourneyAsync(JourneyItem itemDto)
    {
        var item = new JourneyItem
        {
            Title = itemDto.Title,
            Period = itemDto.Period,
            Org = itemDto.Org,
            Description = itemDto.Description,
            DisplayOrder = itemDto.DisplayOrder
        };
        
        _context.JourneyItems.Add(item);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<JourneyItem> { Success = true, Message = "Journey item created", Data = item };
    }
    
    public async Task<ApiResponse<JourneyItem>> UpdateJourneyAsync(JourneyItem itemDto)
    {
        var item = await _context.JourneyItems.FindAsync(itemDto.Id);
        if (item == null)
            return new ApiResponse<JourneyItem> { Success = false, Message = "Journey item not found" };
        
        item.Title = itemDto.Title;
        item.Period = itemDto.Period;
        item.Org = itemDto.Org;
        item.Description = itemDto.Description;
        item.DisplayOrder = itemDto.DisplayOrder;
        
        await _context.SaveChangesAsync();
        
        return new ApiResponse<JourneyItem> { Success = true, Message = "Journey item updated", Data = item };
    }
    
    public async Task<ApiResponse> DeleteJourneyAsync(int id)
    {
        var item = await _context.JourneyItems.FindAsync(id);
        if (item == null)
            return new ApiResponse { Success = false, Message = "Journey item not found" };
        
        _context.JourneyItems.Remove(item);
        await _context.SaveChangesAsync();
        
        return new ApiResponse { Success = true, Message = "Journey item deleted" };
    }
    
    public async Task<ApiResponse<Contact>> UpdateContactAsync(Contact contactDto)
    {
        var contact = await _context.Contacts.FirstOrDefaultAsync();
        if (contact == null)
        {
            contact = new Contact { Id = 0 };
            _context.Contacts.Add(contact);
        }
        
        contact.Email = contactDto.Email;
        contact.WhatsApp = contactDto.WhatsApp;
        contact.Phone = contactDto.Phone;
        contact.Location = contactDto.Location;
        
        await _context.SaveChangesAsync();
        
        return new ApiResponse<Contact> { Success = true, Message = "Contact updated", Data = contact };
    }
    
    public async Task<ApiResponse<SocialLink>> CreateSocialAsync(SocialLink socialDto)
    {
        var social = new SocialLink
        {
            Label = socialDto.Label,
            Href = socialDto.Href,
            Icon = socialDto.Icon
        };
        
        _context.SocialLinks.Add(social);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<SocialLink> { Success = true, Message = "Social link created", Data = social };
    }
    
    public async Task<ApiResponse<SocialLink>> UpdateSocialAsync(SocialLink socialDto)
    {
        var social = await _context.SocialLinks.FindAsync(socialDto.Id);
        if (social == null)
            return new ApiResponse<SocialLink> { Success = false, Message = "Social link not found" };
        
        social.Label = socialDto.Label;
        social.Href = socialDto.Href;
        social.Icon = socialDto.Icon;
        
        await _context.SaveChangesAsync();
        
        return new ApiResponse<SocialLink> { Success = true, Message = "Social link updated", Data = social };
    }
    
    public async Task<ApiResponse> DeleteSocialAsync(int id)
    {
        var social = await _context.SocialLinks.FindAsync(id);
        if (social == null)
            return new ApiResponse { Success = false, Message = "Social link not found" };
        
        _context.SocialLinks.Remove(social);
        await _context.SaveChangesAsync();
        
        return new ApiResponse { Success = true, Message = "Social link deleted" };
    }
    
    public async Task<ApiResponse<SkillCategory>> CreateSkillCategoryAsync(SkillCategory categoryDto)
    {
        var category = new SkillCategory
        {
            Title = categoryDto.Title,
            Color = categoryDto.Color,
            DisplayOrder = categoryDto.DisplayOrder
        };
        
        _context.SkillCategories.Add(category);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<SkillCategory> { Success = true, Message = "Skill category created", Data = category };
    }
    
    public async Task<ApiResponse<SkillCategory>> UpdateSkillCategoryAsync(SkillCategory categoryDto)
    {
        var category = await _context.SkillCategories.FindAsync(categoryDto.Id);
        if (category == null)
            return new ApiResponse<SkillCategory> { Success = false, Message = "Skill category not found" };
        
        category.Title = categoryDto.Title;
        category.Color = categoryDto.Color;
        category.DisplayOrder = categoryDto.DisplayOrder;
        
        await _context.SaveChangesAsync();
        
        return new ApiResponse<SkillCategory> { Success = true, Message = "Skill category updated", Data = category };
    }
    
    public async Task<ApiResponse<Skill>> CreateSkillAsync(Skill skillDto)
    {
        var skill = new Skill
        {
            CategoryId = skillDto.CategoryId,
            Name = skillDto.Name,
            Level = skillDto.Level
        };
        
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        
        return new ApiResponse<Skill> { Success = true, Message = "Skill created", Data = skill };
    }
    
    public async Task<ApiResponse> DeleteSkillAsync(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null)
            return new ApiResponse { Success = false, Message = "Skill not found" };
        
        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
        
        return new ApiResponse { Success = true, Message = "Skill deleted" };
    }
    
    private ProjectDto MapProject(Project p)
    {
        return new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Year = p.Year,
            Category = p.Category,
            Description = p.Description,
            Stack = string.IsNullOrEmpty(p.Stack) 
                ? new List<string>() 
                : JsonSerializer.Deserialize<List<string>>(p.Stack) ?? new List<string>(),
            Image = p.Image,
            LiveUrl = p.LiveUrl,
            GithubUrl = p.GithubUrl,
            Status = p.Status,
            Color = p.Color,
            IsFeatured = p.IsFeatured
        };
    }
}
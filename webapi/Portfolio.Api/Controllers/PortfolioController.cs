using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly PortfolioService _portfolioService;
    private readonly AppDbContext _context;
    
    public PortfolioController(PortfolioService portfolioService, AppDbContext context)
    {
        _portfolioService = portfolioService;
        _context = context;
    }
    
    [HttpGet("config")]
    public async Task<IActionResult> GetConfig()
    {
        var config = await _portfolioService.GetFullConfigAsync();
        return Ok(config);
    }
    
    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills()
    {
        var skills = await _portfolioService.GetSkillCategoriesAsync();
        return Ok(skills);
    }
    
    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _portfolioService.GetAllProjectsAsync();
        return Ok(projects);
    }
    
    [Authorize]
    [HttpPut("hero")]
    public async Task<IActionResult> UpdateHero([FromBody] Hero hero)
    {
        var response = await _portfolioService.UpdateHeroAsync(hero);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("about")]
    public async Task<IActionResult> UpdateAbout([FromBody] About about)
    {
        var response = await _portfolioService.UpdateAboutAsync(about);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("projects")]
    public async Task<IActionResult> CreateProject([FromBody] Project project)
    {
        var response = await _portfolioService.CreateProjectAsync(project);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("projects")]
    public async Task<IActionResult> UpdateProject([FromBody] Project project)
    {
        var response = await _portfolioService.UpdateProjectAsync(project);
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var response = await _portfolioService.DeleteProjectAsync(id);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("journey")]
    public async Task<IActionResult> CreateJourney([FromBody] JourneyItem item)
    {
        var response = await _portfolioService.CreateJourneyAsync(item);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("journey")]
    public async Task<IActionResult> UpdateJourney([FromBody] JourneyItem item)
    {
        var response = await _portfolioService.UpdateJourneyAsync(item);
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("journey/{id}")]
    public async Task<IActionResult> DeleteJourney(int id)
    {
        var response = await _portfolioService.DeleteJourneyAsync(id);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("contact")]
    public async Task<IActionResult> UpdateContact([FromBody] Contact contact)
    {
        var response = await _portfolioService.UpdateContactAsync(contact);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("socials")]
    public async Task<IActionResult> CreateSocial([FromBody] SocialLink social)
    {
        var response = await _portfolioService.CreateSocialAsync(social);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("socials")]
    public async Task<IActionResult> UpdateSocial([FromBody] SocialLink social)
    {
        var response = await _portfolioService.UpdateSocialAsync(social);
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("socials/{id}")]
    public async Task<IActionResult> DeleteSocial(int id)
    {
        var response = await _portfolioService.DeleteSocialAsync(id);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("skills/categories")]
    public async Task<IActionResult> CreateSkillCategory([FromBody] SkillCategory category)
    {
        var response = await _portfolioService.CreateSkillCategoryAsync(category);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("skills/categories")]
    public async Task<IActionResult> UpdateSkillCategory([FromBody] SkillCategory category)
    {
        var response = await _portfolioService.UpdateSkillCategoryAsync(category);
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill([FromBody] Skill skill)
    {
        var response = await _portfolioService.CreateSkillAsync(skill);
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("skills/{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var response = await _portfolioService.DeleteSkillAsync(id);
        return Ok(response);
    }
    
    // GET: api/portfolio/dashboard-stats
    [HttpGet("dashboard-stats")]
    [Authorize]
    public async Task<IActionResult> GetDashboardStats()
    {
        var totalProjects = await _context.Projects.CountAsync();
        var draftProjects = await _context.Projects.CountAsync(p => p.Status == "Draft");
        var unreadMessages = await _context.Messages.CountAsync(m => !m.IsRead);
        var totalSkills = await _context.Skills.CountAsync();
        var skillCategories = await _context.SkillCategories.CountAsync();
        
        // Mock profile views - in production, this would come from analytics
        var random = new Random();
        var profileViews = 12450 + random.Next(-100, 100);
        
        return Ok(new
        {
            totalProjects,
            draftProjects,
            unreadMessages,
            totalSkills,
            skillCategories,
            profileViews,
            recentProjects = await _context.Projects
                .OrderByDescending(p => p.Id)
                .Take(5)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Description,
                    p.Stack,
                    p.Status,
                    p.Image
                })
                .ToListAsync()
        });
    }
}

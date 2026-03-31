using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Application.Features.Portfolio.Queries;
using PortfolioApi.Application.Features.Portfolio.Commands;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly AppDbContext _context;
    
    public PortfolioController(ISender mediator, AppDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    // Public endpoint - only return published projects
    [HttpGet("projects")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProjects()
    {
        var projects = await _context.Projects
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.Id)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Description,
                p.Stack,
                p.Status,
                p.Image,
                p.Year,
                p.Category,
                p.LiveUrl,
                p.GithubUrl,
                p.Color,
                p.IsFeatured,
                p.ViewsCount
            })
            .ToListAsync();
        return Ok(projects);
    }

    // PUT /api/portfolio/projects/{id}/publish
    [HttpPut("projects/{id}/publish")]
    [Authorize]
    public async Task<IActionResult> PublishProject(int id)
    {
        var result = await _mediator.Send(new PublishProjectCommand(id));
        return result ? Ok() : NotFound();
    }

    // PUT /api/portfolio/projects/{id}/unpublish
    [HttpPut("projects/{id}/unpublish")]
    [Authorize]
    public async Task<IActionResult> UnpublishProject(int id)
    {
        var result = await _mediator.Send(new UnpublishProjectCommand(id));
        return result ? Ok() : NotFound();
    }
    
    [HttpGet("config")]
    public async Task<IActionResult> GetConfig()
    {
        var config = await _mediator.Send(new GetFullConfigQuery());
        return Ok(config);
    }
    
    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills()
    {
        var skills = await _mediator.Send(new GetSkillCategoriesQuery());
        return Ok(skills);
    }
    
    [Authorize]
    [HttpPut("hero")]
    public async Task<IActionResult> UpdateHero([FromBody] Hero hero)
    {
        var response = await _mediator.Send(new UpdateHeroCommand(hero));
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("about")]
    public async Task<IActionResult> UpdateAbout([FromBody] About about)
    {
        var response = await _mediator.Send(new UpdateAboutCommand(about));
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("projects")]
    public async Task<IActionResult> CreateProject([FromBody] Project project)
    {
        var response = await _mediator.Send(new CreateProjectCommand(project));
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("projects")]
    public async Task<IActionResult> UpdateProject([FromBody] Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return Ok(project);
    }
    
    [Authorize]
    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return NotFound();
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [Authorize]
    [HttpPost("journey")]
    public async Task<IActionResult> CreateJourney([FromBody] JourneyItem item)
    {
        _context.JourneyItems.Add(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }
    
    [Authorize]
    [HttpPut("journey")]
    public async Task<IActionResult> UpdateJourney([FromBody] JourneyItem item)
    {
        _context.JourneyItems.Update(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }
    
    [Authorize]
    [HttpDelete("journey/{id}")]
    public async Task<IActionResult> DeleteJourney(int id)
    {
        var item = await _context.JourneyItems.FindAsync(id);
        if (item == null) return NotFound();
        _context.JourneyItems.Remove(item);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [Authorize]
    [HttpPut("contact")]
    public async Task<IActionResult> UpdateContact([FromBody] Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
        return Ok(contact);
    }
    
    [Authorize]
    [HttpPost("socials")]
    public async Task<IActionResult> CreateSocial([FromBody] SocialLink social)
    {
        _context.SocialLinks.Add(social);
        await _context.SaveChangesAsync();
        return Ok(social);
    }
    
    [Authorize]
    [HttpPut("socials")]
    public async Task<IActionResult> UpdateSocial([FromBody] SocialLink social)
    {
        _context.SocialLinks.Update(social);
        await _context.SaveChangesAsync();
        return Ok(social);
    }
    
    [Authorize]
    [HttpDelete("socials/{id}")]
    public async Task<IActionResult> DeleteSocial(int id)
    {
        var social = await _context.SocialLinks.FindAsync(id);
        if (social == null) return NotFound();
        _context.SocialLinks.Remove(social);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [Authorize]
    [HttpPost("skills/categories")]
    public async Task<IActionResult> CreateSkillCategory([FromBody] SkillCategory category)
    {
        _context.SkillCategories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }
    
    [Authorize]
    [HttpPut("skills/categories")]
    public async Task<IActionResult> UpdateSkillCategory([FromBody] SkillCategory category)
    {
        _context.SkillCategories.Update(category);
        await _context.SaveChangesAsync();
        return Ok(category);
    }
    
    [Authorize]
    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill([FromBody] Skill skill)
    {
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        return Ok(skill);
    }
    
    [Authorize]
    [HttpDelete("skills/{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null) return NotFound();
        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
        return Ok();
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

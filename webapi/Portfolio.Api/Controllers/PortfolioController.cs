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
    private readonly ILogger<PortfolioController> _logger;
    private readonly AppDbContext _context;
    
    public PortfolioController(ISender mediator, ILogger<PortfolioController> logger, AppDbContext context)
    {
        _mediator = mediator;
        _logger = logger;
        _context = context;
    }

    // Public endpoint - only return published projects
    [HttpGet("projects")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProjects()
    {
        _logger.LogInformation("API: Getting public projects");
        var projects = await _mediator.Send(new GetPublicProjectsQuery());
        return Ok(projects);
    }

    // Admin endpoint - return all projects including drafts
    [HttpGet("admin/projects")]
    [Authorize]
    public async Task<IActionResult> GetAllProjectsAdmin()
    {
        _logger.LogInformation("API: Admin getting all projects");
        var projects = await _mediator.Send(new GetAllProjectsAdminQuery());
        return Ok(projects);
    }

    // PUT /api/portfolio/projects/{id}/publish
    [HttpPut("projects/{id}/publish")]
    [Authorize]
    public async Task<IActionResult> PublishProject(int id)
    {
        _logger.LogInformation("API: Publishing project {ProjectId}", id);
        var result = await _mediator.Send(new PublishProjectCommand(id));
        return result ? Ok() : NotFound();
    }

    // PUT /api/portfolio/projects/{id}/unpublish
    [HttpPut("projects/{id}/unpublish")]
    [Authorize]
    public async Task<IActionResult> UnpublishProject(int id)
    {
        _logger.LogInformation("API: Unpublishing project {ProjectId}", id);
        var result = await _mediator.Send(new UnpublishProjectCommand(id));
        return result ? Ok() : NotFound();
    }
    
    [HttpGet("config")]
    public async Task<IActionResult> GetConfig()
    {
        _logger.LogInformation("API: Getting portfolio config");
        var config = await _mediator.Send(new GetFullConfigQuery());
        return Ok(config);
    }
    
    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills()
    {
        _logger.LogInformation("API: Getting skills");
        var skills = await _mediator.Send(new GetSkillCategoriesQuery());
        return Ok(skills);
    }
    
    [Authorize]
    [HttpPut("hero")]
    public async Task<IActionResult> UpdateHero([FromBody] Hero hero)
    {
        _logger.LogInformation("API: Updating hero section");
        var response = await _mediator.Send(new UpdateHeroCommand(hero));
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("about")]
    public async Task<IActionResult> UpdateAbout([FromBody] About about)
    {
        _logger.LogInformation("API: Updating about section");
        var response = await _mediator.Send(new UpdateAboutCommand(about));
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("projects")]
    public async Task<IActionResult> CreateProject([FromBody] Project project)
    {
        _logger.LogInformation("API: Creating new project: {Title}", project.Title);
        var response = await _mediator.Send(new CreateProjectCommand(project));
        _logger.LogInformation("API: Project created successfully");
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("projects")]
    public async Task<IActionResult> UpdateProject([FromBody] Project project)
    {
        _logger.LogInformation("API: Updating project {ProjectId}", project.Id);
        var response = await _mediator.Send(new UpdateProjectCommand { Project = project });
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("projects/{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        _logger.LogInformation("API: Deleting project {ProjectId}", id);
        var result = await _mediator.Send(new DeleteProjectCommand { Id = id });
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpPost("journey")]
    public async Task<IActionResult> CreateJourney([FromBody] JourneyItem item)
    {
        _logger.LogInformation("API: Creating journey item: {Title}", item.Title);
        var response = await _mediator.Send(new CreateJourneyCommand { Item = item });
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("journey")]
    public async Task<IActionResult> UpdateJourney([FromBody] JourneyItem item)
    {
        _logger.LogInformation("API: Updating journey item {JourneyId}", item.Id);
        var response = await _mediator.Send(new UpdateJourneyCommand { Item = item });
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("journey/{id}")]
    public async Task<IActionResult> DeleteJourney(int id)
    {
        _logger.LogInformation("API: Deleting journey item {JourneyId}", id);
        var result = await _mediator.Send(new DeleteJourneyCommand { Id = id });
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpPut("contact")]
    public async Task<IActionResult> UpdateContact([FromBody] Contact contact)
    {
        _logger.LogInformation("API: Updating contact information");
        var response = await _mediator.Send(new UpdateContactCommand { Contact = contact });
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("socials")]
    public async Task<IActionResult> CreateSocial([FromBody] SocialLink social)
    {
        _logger.LogInformation("API: Creating social link: {Label}", social.Label);
        var response = await _mediator.Send(new CreateSocialCommand { Social = social });
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("socials")]
    public async Task<IActionResult> UpdateSocial([FromBody] SocialLink social)
    {
        _logger.LogInformation("API: Updating social link {SocialId}", social.Id);
        var response = await _mediator.Send(new UpdateSocialCommand { Social = social });
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("socials/{id}")]
    public async Task<IActionResult> DeleteSocial(int id)
    {
        _logger.LogInformation("API: Deleting social link {SocialId}", id);
        var result = await _mediator.Send(new DeleteSocialCommand { Id = id });
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpPost("skills/categories")]
    public async Task<IActionResult> CreateSkillCategory([FromBody] SkillCategory category)
    {
        _logger.LogInformation("API: Creating skill category: {Title}", category.Title);
        var response = await _mediator.Send(new CreateSkillCategoryCommand { Category = category });
        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("skills/categories")]
    public async Task<IActionResult> UpdateSkillCategory([FromBody] SkillCategory category)
    {
        _logger.LogInformation("API: Updating skill category {CategoryId}", category.Id);
        var response = await _mediator.Send(new UpdateSkillCategoryCommand { Category = category });
        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("skills")]
    public async Task<IActionResult> CreateSkill([FromBody] Skill skill)
    {
        _logger.LogInformation("API: Creating skill: {Name}", skill.Name);
        var response = await _mediator.Send(new CreateSkillCommand { Skill = skill });
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("skills/{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        _logger.LogInformation("API: Deleting skill {SkillId}", id);
        var result = await _mediator.Send(new DeleteSkillCommand { Id = id });
        return result ? Ok() : NotFound();
    }
    
    // GET: api/portfolio/dashboard-stats
    [HttpGet("dashboard-stats")]
    [Authorize]
    public async Task<IActionResult> GetDashboardStats()
    {
        _logger.LogInformation("API: Getting dashboard stats");
        var stats = await _mediator.Send(new GetDashboardStatsQuery());
        return Ok(stats);
    }

    // GET: api/portfolio/cv
    [HttpGet("cv")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCV()
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == "cv_url");
        
        if (string.IsNullOrEmpty(setting?.Value))
            return NotFound("CV not found");
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", setting.Value.TrimStart('/'));
        
        if (!System.IO.File.Exists(filePath))
            return NotFound("CV file not found");
        
        return PhysicalFile(filePath, "application/pdf", "Abdullah_Mohammed_CV.pdf");
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Application.Features.Portfolio.Queries;
using PortfolioApi.Application.Features.Portfolio.Commands;
using PortfolioApi.Application.Features.Projects.Commands;
using PortfolioApi.Infrastructure.Data;
using SQLitePCL;
using System.Net.Http;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ILogger<PortfolioController> _logger;
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    
    public PortfolioController(ISender mediator, ILogger<PortfolioController> logger, AppDbContext context, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _logger = logger;
        _context = context;
        _environment = environment;
    }

    // Public endpoint - only return published projects
    [HttpGet("projects")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetPublicProjects()
    {
        _logger.LogInformation("API: Getting public projects");
        var projects = await _mediator.Send(new GetPublicProjectsQuery());
        return Ok(new ApiResponse<object> { Success = true, Data = projects });
    }

    // Admin endpoint - return all projects including drafts
    [HttpGet("admin/projects")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetAllProjectsAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("API: Admin getting projects page {Page} with size {PageSize}", page, pageSize);
        var result = await _mediator.Send(new GetAllProjectsAdminQuery { Page = page, PageSize = pageSize });
        return Ok(new ApiResponse<object> { Success = true, Data = result });
    }

    // POST /api/portfolio/projects/{id}/views
    [HttpPost("projects/{id}/views")]
    [AllowAnonymous]
    public async Task<IActionResult> IncrementProjectViews(int id)
    {
        _logger.LogInformation("API: Incrementing views for project {ProjectId}", id);
        var result = await _mediator.Send(new IncrementProjectViewsCommand(id));
        return result ? Ok() : NotFound();
    }

   
    
    [HttpGet("config")]
    public async Task<ActionResult<ApiResponse<object>>> GetConfig()
    {
        _logger.LogInformation("API: Getting portfolio config");
        var config = await _mediator.Send(new GetFullConfigQuery());
        return Ok(new ApiResponse<object> { Success = true, Data = config });
    }
    
    [HttpGet("skills")]
    public async Task<ActionResult<ApiResponse<object>>> GetSkills()
    {
        _logger.LogInformation("API: Getting skills");
        var skills = await _mediator.Send(new GetSkillCategoriesQuery());
        return Ok(new ApiResponse<object> { Success = true, Data = skills });
    }
    [HttpGet("ProfileImage")]
    public async Task<ActionResult<ApiResponse<string>>> GetProfileImage()
    {
        var imagePath = await _mediator.Send(new GetProfileImageQuery());
        return Ok(new ApiResponse<string> { Success = true, Data = imagePath });
    }
    [Authorize]
    [HttpPut("hero")]
    public async Task<IActionResult> UpdateHero([FromBody] UpdateHeroRequest request)
    {
        _logger.LogInformation("API: Updating hero section");
        var response = await _mediator.Send(new UpdateHeroCommand(request.Hero, request.Stats));
        return Ok(response);
    }
    
    [HttpGet("dashboard-stats")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> GetDashboardStats()
    {
        _logger.LogInformation("API: Getting dashboard stats");
        var stats = await _mediator.Send(new GetDashboardStatsQuery());
        return Ok(new ApiResponse<object> { Success = true, Data = stats });
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
    [HttpPut("projects/{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
    {
        if (id != project.Id)
            return BadRequest(new ApiResponse { Success = false, Message = "ID mismatch" });
            
        _logger.LogInformation("API: Updating project {ProjectId}", project.Id);
        var response = await _mediator.Send(new UpdateProjectCommand { Project = project });
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("projects/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteProject(int id)
    {
        _logger.LogInformation("API: Deleting project {ProjectId}", id);
        var result = await _mediator.Send(new DeleteProjectCommand { Id = id });
        if (!result)
            return NotFound(new ApiResponse { Success = false, Message = "Project not found" });
        return Ok(new ApiResponse { Success = true, Message = "Project deleted" });
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
     public async Task<ActionResult<ApiResponse>> DeleteJourney(int id)
     {
         _logger.LogInformation("API: Deleting journey item {JourneyId}", id);
         var result = await _mediator.Send(new DeleteJourneyCommand { Id = id });
         if (!result)
             return NotFound(new ApiResponse { Success = false, Message = "Journey item not found" });
         return Ok(new ApiResponse { Success = true, Message = "Journey item deleted" });
     }
     
     [HttpGet("journey")]
     [Authorize]
     public async Task<IActionResult> GetJourney()
     {
         _logger.LogInformation("API: Getting journey items");
         var journey = await _mediator.Send(new GetJourneyQuery());
         return Ok(journey);
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
    public async Task<ActionResult<ApiResponse>> DeleteSocial(int id)
    {
        _logger.LogInformation("API: Deleting social link {SocialId}", id);
        var result = await _mediator.Send(new DeleteSocialCommand { Id = id });
        if (!result)
            return NotFound(new ApiResponse { Success = false, Message = "Social link not found" });
        return Ok(new ApiResponse { Success = true, Message = "Social link deleted" });
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
    [HttpPut("skills")]
    public async Task<IActionResult> UpdateSkill([FromBody] Skill skill)
    {
        _logger.LogInformation("API: Updating skill {SkillId}: {Name}", skill.Id, skill.Name);
        var response = await _mediator.Send(new UpdateSkillCommand { Skill = skill });
        return Ok(response);
    }
    
    [Authorize]
    [HttpDelete("skills/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteSkill(int id)
    {
        _logger.LogInformation("API: Deleting skill {SkillId}", id);
        var result = await _mediator.Send(new DeleteSkillCommand { Id = id });
        if (!result)
            return NotFound(new ApiResponse { Success = false, Message = "Skill not found" });
        return Ok(new ApiResponse { Success = true, Message = "Skill deleted" });
    }
    
    [Authorize]
    [HttpDelete("skills/categories/{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteSkillCategory(int id)
    {
        _logger.LogInformation("API: Deleting skill category {CategoryId}", id);
        var result = await _mediator.Send(new DeleteSkillCategoryCommand { Id = id });
        if (!result)
            return NotFound(new ApiResponse { Success = false, Message = "Skill category not found" });
        return Ok(new ApiResponse { Success = true, Message = "Skill category deleted" });
    }
   

    // GET: api/portfolio/cv
    [HttpGet("cv")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCV()
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "cv_url");
        if (setting == null || string.IsNullOrEmpty(setting.Value))
            return NotFound("CV not configured");
        
        var fileName = Path.GetFileName(setting.Value);
        
        // Debug: Log all path info
        _logger.LogInformation("=== CV DEBUG ===");
        _logger.LogInformation("ContentRootPath: {Path}", _environment.ContentRootPath);
        _logger.LogInformation("WebRootPath: {Path}", _environment.WebRootPath);
        _logger.LogInformation("Setting.Value: {Value}", setting.Value);
        _logger.LogInformation("FileName extracted: {FileName}", fileName);
        
        // Handle MonsterASP wwwroot/wwwroot structure
        // Try different path combinations
        var possiblePaths = new[]
        {
            Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "cv", fileName),
            Path.Combine(_environment.WebRootPath, "uploads", "cv", fileName),
            Path.Combine(_environment.ContentRootPath, "uploads", "cv", fileName),
            $"/wwwroot/wwwroot/uploads/cv/{fileName}",  // Absolute path for MonsterASP
            $"C:/home/site/wwwroot/wwwroot/uploads/cv/{fileName}".Replace('/', Path.DirectorySeparatorChar)
        };
        
        string foundPath = null;
        foreach (var path in possiblePaths)
        {
            _logger.LogInformation("Checking path: {Path}, Exists: {Exists}", path, System.IO.File.Exists(path));
            if (System.IO.File.Exists(path))
            {
                foundPath = path;
                _logger.LogInformation("FOUND CV at: {Path}", path);
                break;
            }
        }
        
        if (foundPath == null)
        {
            _logger.LogWarning("CV file not found in any location");
            return NotFound("CV file not found. Please upload CV via admin panel.");
        }
        
        var filePath = foundPath;
        
        // Add cache-busting using file timestamp
        var lastModified = System.IO.File.GetLastWriteTime(filePath);
        var etag = lastModified.ToString("yyyyMMddHHmmss");
        
        Response.Headers["ETag"] = etag;
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";
        
        return PhysicalFile(filePath, "application/pdf", fileName);
    }

    // GET: api/portfolio/cv/preview
    [HttpGet("cv/preview")]
    [AllowAnonymous]
    public async Task<IActionResult> PreviewCV()
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "cv_url");
        if (setting == null || string.IsNullOrEmpty(setting.Value))
            return NotFound("CV not configured");

        try
        {
            var fileName = Path.GetFileName(setting.Value);
            
            // If it's a Cloudinary URL (legacy), redirect to download endpoint
            if (setting.Value.StartsWith("http"))
            {
                return Redirect("/api/portfolio/cv");
            }
            
            // Debug: Log all path info
            _logger.LogInformation("=== CV PREVIEW DEBUG ===");
            _logger.LogInformation("ContentRootPath: {Path}", _environment.ContentRootPath);
            _logger.LogInformation("WebRootPath: {Path}", _environment.WebRootPath);
            _logger.LogInformation("FileName: {FileName}", fileName);
            
            // Try different path combinations
            var possiblePaths = new[]
            {
                Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "cv", fileName),
                Path.Combine(_environment.WebRootPath, "uploads", "cv", fileName),
                Path.Combine(_environment.ContentRootPath, "uploads", "cv", fileName),
                $"/wwwroot/wwwroot/uploads/cv/{fileName}",
                $"C:/home/site/wwwroot/wwwroot/uploads/cv/{fileName}".Replace('/', Path.DirectorySeparatorChar)
            };
            
            string foundPath = null;
            foreach (var path in possiblePaths)
            {
                _logger.LogInformation("Checking path: {Path}, Exists: {Exists}", path, System.IO.File.Exists(path));
                if (System.IO.File.Exists(path))
                {
                    foundPath = path;
                    _logger.LogInformation("FOUND CV at: {Path}", path);
                    break;
                }
            }
            
            if (foundPath == null)
            {
                _logger.LogWarning("CV file not found in any location");
                return NotFound("CV file not found. Please upload CV via admin panel.");
            }
            
            var filePath = foundPath;
            
            // Read file and serve inline
            var content = await System.IO.File.ReadAllBytesAsync(filePath);
            
            // Set headers for inline PDF viewing
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            
            return File(content, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing CV from local storage");
            return StatusCode(500, "Error loading CV preview");
        }
    }
}

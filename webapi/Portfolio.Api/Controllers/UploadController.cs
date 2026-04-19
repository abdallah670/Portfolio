using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Features.Portfolio.Commands;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadController> _logger;
    private readonly IMediator _mediator;
    
    public UploadController(AppDbContext context, IWebHostEnvironment environment, ILogger<UploadController> logger, IMediator mediator)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
        _mediator = mediator;
    }
    
    [HttpPost("project-image")]
    public async Task<IActionResult> UploadProjectImage( IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });
        
        _logger.LogInformation("Uploaded file: {FileName}, ContentType: {ContentType}, Size: {Size}", 
            file.FileName, file.ContentType, file.Length);
            
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        
        if (string.IsNullOrEmpty(extension))
            return BadRequest(new ApiResponse { Success = false, Message = "No file extension found" });
        
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new ApiResponse { Success = false, Message = $"Invalid file type: {extension}. Allowed: jpg, jpeg, png, gif, webp, svg" });
            
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new ApiResponse { Success = false, Message = "File too large. Max 5MB" });
            
        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "projects");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);
            
        var filePath = Path.Combine(uploadsFolder, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        //upadte path in hero
     
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var imageUrl = $"{baseUrl}/uploads/projects/{fileName}";
        
        return Ok(new ApiResponse<string> { Success = true, Message = "Image uploaded", Data = imageUrl });
    }
    
    [HttpPost("profile-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });
            
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid file type" });
            
        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads/profile-image");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);
        // Delete old profile image if exists
        var existingFiles = Directory.GetFiles(uploadsFolder);
        foreach (var f in existingFiles) System.IO.File.Delete(f);
        var filePath = Path.Combine(uploadsFolder, fileName);
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
            
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        var response = await _mediator.Send(new UpdateHeroImageCommand { ImagePath = $"/uploads/profile-image/{fileName}" });
        if (!response.Success || response.Data == null)
        {
            _logger.LogError("Failed to update hero image in database");
            return StatusCode(500, new ApiResponse { Success = false, Message = "Image uploaded but failed to update profile" });
        }
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var imageUrl = $"{baseUrl}/uploads/profile-image/{fileName}";
        
        return Ok(new ApiResponse<string> { Success = true, Message = "Image uploaded", Data = imageUrl });
    }

    [HttpPost("cv")]
    [Authorize]
    public async Task<IActionResult> UploadCV(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });
        
        if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ApiResponse { Success = false, Message = "Only PDF files are allowed" });
        
        if (file.Length > 10 * 1024 * 1024) // 10MB max
            return BadRequest(new ApiResponse { Success = false, Message = "File too large. Max 10MB" });
        
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "cv");
        Directory.CreateDirectory(uploadsFolder);
        
        // Delete old CV if exists
        var existingFiles = Directory.GetFiles(uploadsFolder, "*.pdf");
        foreach (var f in existingFiles) System.IO.File.Delete(f);
        
        var fileName = $"Abdullah_Mohammed_CV.pdf";
        var filePath = Path.Combine(uploadsFolder, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        // Save path to SystemSettings
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "cv_url");
        if (setting == null)
        {
            setting = new SystemSetting { Key = "cv_url", Category = "files", DataType = "string" };
            _context.SystemSettings.Add(setting);
        }
        setting.Value = $"/uploads/cv/{fileName}";
        await _context.SaveChangesAsync();
        
        return Ok(new ApiResponse<string> { Success = true, Message = "CV uploaded", Data = setting.Value });
    }
    [HttpDelete("file/{*filePath}")]
    [Authorize]
    public async Task<IActionResult> DeleteFile([FromRoute] string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return BadRequest(new ApiResponse { Success = false, Message = "File path is required" });
        
        var fullPath = Path.Combine(_environment.ContentRootPath, "wwwroot", filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        
        if (!System.IO.File.Exists(fullPath))
            return NotFound(new ApiResponse { Success = false, Message = "File not found" });
        
        System.IO.File.Delete(fullPath);
        
        return Ok(new ApiResponse { Success = true, Message = "File deleted" });
    }
}


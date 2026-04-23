using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Features.Portfolio.Commands;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;
using PortfolioApi.Infrastructure.Services;

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
    public async Task<IActionResult> UploadProjectImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid file type. Only JPG, PNG, GIF, WebP allowed." });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new ApiResponse { Success = false, Message = "File size exceeds 5MB limit" });

        _logger.LogInformation("Uploading project image to local storage: {FileName}", file.FileName);

        try
        {
            // Ensure directory exists
            var projectsFolder = Path.Combine(_environment.WebRootPath, "uploads", "projects");
            if (!Directory.Exists(projectsFolder))
                Directory.CreateDirectory(projectsFolder);

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(projectsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Project image saved: {Path}", filePath);

            // Return relative URL
            var imageUrl = $"/uploads/projects/{fileName}";
            return Ok(new ApiResponse<string> { Success = true, Message = "Image uploaded", Data = imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload project image");
            return BadRequest(new ApiResponse { Success = false, Message = $"Upload failed: {ex.Message}" });
        }
    }
    
    [HttpPost("profile-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid file type. Only JPG, PNG, GIF, WebP allowed." });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new ApiResponse { Success = false, Message = "File size exceeds 5MB limit" });

        _logger.LogInformation("Uploading profile image to local storage: {FileName}", file.FileName);

        try
        {
            // Ensure directory exists
            var profileFolder = Path.Combine(_environment.WebRootPath, "uploads", "profile-image");
            if (!Directory.Exists(profileFolder))
                Directory.CreateDirectory(profileFolder);

            // Delete old profile image(s)
            var existingFiles = Directory.GetFiles(profileFolder);
            foreach (var f in existingFiles)
            {
                System.IO.File.Delete(f);
                _logger.LogInformation("Deleted old profile image: {File}", f);
            }

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(profileFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Profile image saved: {Path}", filePath);

            // Return relative URL
            var imageUrl = $"/uploads/profile-image/{fileName}";

            // Update database with local URL
            var response = await _mediator.Send(new UpdateHeroImageCommand { ImagePath = imageUrl });
            if (!response.Success || response.Data == null)
            {
                _logger.LogError("Failed to update hero image in database");
                return StatusCode(500, new ApiResponse { Success = false, Message = "Image uploaded but failed to update profile" });
            }

            return Ok(new ApiResponse<string> { Success = true, Message = "Image uploaded", Data = imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload profile image");
            return BadRequest(new ApiResponse { Success = false, Message = $"Upload failed: {ex.Message}" });
        }
    }

    [HttpPost("cv")]
    [Authorize]
    public async Task<IActionResult> UploadCV(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });

        // Validate file type
        if (file.ContentType.ToLower() != "application/pdf" &&
            !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiResponse { Success = false, Message = "Only PDF files are allowed for CV upload" });
        }

        // Validate file size (10MB max)
        if (file.Length > 10 * 1024 * 1024)
        {
            return BadRequest(new ApiResponse { Success = false, Message = "File size exceeds 10MB limit" });
        }

        _logger.LogInformation("Uploading CV to local storage: {FileName}", file.FileName);

        try
        {
            // Ensure CV directory exists (MonsterASP uses WebRootPath = wwwroot/wwwroot)
            var cvFolder = Path.Combine(_environment.WebRootPath, "uploads", "cv");
            if (!Directory.Exists(cvFolder))
            {
                Directory.CreateDirectory(cvFolder);
            }

            // Delete old CV if exists
            var existingFiles = Directory.GetFiles(cvFolder, "*.pdf");
            foreach (var f in existingFiles)
            {
                System.IO.File.Delete(f);
                _logger.LogInformation("Deleted old CV: {File}", f);
            }

            // Save new CV
            var fileName = "Abdullah_Mohammed_CV.pdf";
            var filePath = Path.Combine(cvFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("CV saved to local storage: {Path}", filePath);

            // Build URL - use relative path that works with both local and production
            var cvUrl = $"/uploads/cv/{fileName}";

            // Save to SystemSettings
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "cv_url");
            if (setting == null)
            {
                setting = new SystemSetting { Key = "cv_url", Category = "files", DataType = "string" };
                _context.SystemSettings.Add(setting);
            }
            setting.Value = cvUrl;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string> { Success = true, Message = "CV uploaded", Data = cvUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload CV to local storage");
            return BadRequest(new ApiResponse { Success = false, Message = $"Upload failed: {ex.Message}" });
        }
    }
    [HttpDelete("file/{*filePath}")]
    [Authorize]
    public async Task<IActionResult> DeleteFile([FromRoute] string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return BadRequest(new ApiResponse { Success = false, Message = "File path is required" });
        
        // filePath comes as "/uploads/projects/filename.png" or "/uploads/profile-image/filename.jpg" or "/uploads/cv/filename.pdf"
        var cleanPath = filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_environment.WebRootPath, cleanPath);
        
        _logger.LogInformation("Attempting to delete file at: {FullPath}", fullPath);
        
        if (!System.IO.File.Exists(fullPath))
            return NotFound(new ApiResponse { Success = false, Message = "File not found" });
        
        System.IO.File.Delete(fullPath);
        _logger.LogInformation("File deleted successfully: {FullPath}", fullPath);
        
        return Ok(new ApiResponse { Success = true, Message = "File deleted" });
    }
}


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
    private readonly ICloudinaryService _cloudinaryService;
    
    public UploadController(AppDbContext context, IWebHostEnvironment environment, ILogger<UploadController> logger, IMediator mediator, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
        _mediator = mediator;
        _cloudinaryService = cloudinaryService;
    }
    
    [HttpPost("project-image")]
    public async Task<IActionResult> UploadProjectImage( IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });
        
        _logger.LogInformation("Uploading project image to Cloudinary: {FileName}, ContentType: {ContentType}, Size: {Size}", 
            file.FileName, file.ContentType, file.Length);
        
        try
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(file, "projects");
            _logger.LogInformation("Project image uploaded to Cloudinary: {Url}", imageUrl);
            return Ok(new ApiResponse<string> { Success = true, Message = "Image uploaded", Data = imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload project image to Cloudinary");
            return BadRequest(new ApiResponse { Success = false, Message = $"Upload failed: {ex.Message}" });
        }
    }
    
    [HttpPost("profile-image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });
        
        _logger.LogInformation("Uploading profile image to Cloudinary: {FileName}", file.FileName);
        
        try
        {
            var imageUrl = await _cloudinaryService.UploadImageAsync(file, "profile");
            _logger.LogInformation("Profile image uploaded to Cloudinary: {Url}", imageUrl);
            
            // Update database with Cloudinary URL
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
            _logger.LogError(ex, "Failed to upload profile image to Cloudinary");
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
        
        // filePath comes as "/uploads/cv/filename.pdf"
        var fileName = Path.GetFileName(filePath);
        var fullPath = Path.Combine(_environment.WebRootPath, "uploads", "cv", fileName);
        
        _logger.LogInformation("Attempting to delete file at: {FullPath}", fullPath);
        
        if (!System.IO.File.Exists(fullPath))
            return NotFound(new ApiResponse { Success = false, Message = "File not found" });
        
        System.IO.File.Delete(fullPath);
        _logger.LogInformation("File deleted successfully: {FullPath}", fullPath);
        
        return Ok(new ApiResponse { Success = true, Message = "File deleted" });
    }
}


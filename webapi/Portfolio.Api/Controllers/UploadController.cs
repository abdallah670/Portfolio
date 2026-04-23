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
        
        _logger.LogInformation("Uploading CV to Cloudinary: {FileName}", file.FileName);
        
        try
        {
            var cvUrl = await _cloudinaryService.UploadCVAsync(file);
            _logger.LogInformation("CV uploaded to Cloudinary: {Url}", cvUrl);
            
            // Save Cloudinary URL to SystemSettings
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
            _logger.LogError(ex, "Failed to upload CV to Cloudinary");
            return BadRequest(new ApiResponse { Success = false, Message = $"Upload failed: {ex.Message}" });
        }
    }
    [HttpDelete("file/{*filePath}")]
    [Authorize]
    public async Task<IActionResult> DeleteFile([FromRoute] string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return BadRequest(new ApiResponse { Success = false, Message = "File path is required" });
        
        // If it's a Cloudinary URL, extract public ID and delete from Cloudinary
        if (filePath.Contains("cloudinary.com"))
        {
            try
            {
                // Extract public ID from Cloudinary URL
                var publicId = ExtractCloudinaryPublicId(filePath);
                if (!string.IsNullOrEmpty(publicId))
                {
                    await _cloudinaryService.DeleteFileAsync(publicId);
                    return Ok(new ApiResponse { Success = true, Message = "File deleted from Cloudinary" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file from Cloudinary");
                return BadRequest(new ApiResponse { Success = false, Message = $"Delete failed: {ex.Message}" });
            }
        }
        
        // Fallback to local file deletion
        var fullPath = Path.Combine(_environment.ContentRootPath, "wwwroot", filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        
        if (!System.IO.File.Exists(fullPath))
            return NotFound(new ApiResponse { Success = false, Message = "File not found" });
        
        System.IO.File.Delete(fullPath);
        
        return Ok(new ApiResponse { Success = true, Message = "File deleted" });
    }
    
    private string ExtractCloudinaryPublicId(string cloudinaryUrl)
    {
        try
        {
            // Cloudinary URL format: https://res.cloudinary.com/{cloud}/image/upload/v{version}/{folder}/{publicId}.{ext}
            var uri = new Uri(cloudinaryUrl);
            var segments = uri.Segments;
            if (segments.Length >= 2)
            {
                // Get the last segment (filename without extension)
                var lastSegment = segments[^1];
                var folder = segments.Length > 2 ? segments[^2].Trim('/') : "";
                var publicId = Path.GetFileNameWithoutExtension(lastSegment);
                return string.IsNullOrEmpty(folder) ? publicId : $"{folder}/{publicId}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract Cloudinary public ID from URL: {Url}", cloudinaryUrl);
        }
        return null;
    }
}


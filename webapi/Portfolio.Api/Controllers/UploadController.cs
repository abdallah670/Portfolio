using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
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
    
    public UploadController(AppDbContext context, IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        _context = context;
        _environment = environment;
        _logger = logger;
    }
    
    [HttpPost("project-image")]
    public async Task<IActionResult> UploadProjectImage([FromForm] IFormFile file)
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
            
        var fileName = $"profile{extension}";
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);
            
        var filePath = Path.Combine(uploadsFolder, fileName);
        
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
            
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var imageUrl = $"{baseUrl}/uploads/{fileName}";
        
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
}
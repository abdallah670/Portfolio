using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    
    public UploadController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }
    
    [HttpPost("project-image")]
    public async Task<IActionResult> UploadProjectImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "No file provided" });
            
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid file type. Allowed: jpg, jpeg, png, gif, webp" });
            
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
}
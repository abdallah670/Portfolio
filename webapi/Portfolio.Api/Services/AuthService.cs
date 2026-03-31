using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortfolioApi.Data;
using PortfolioApi.DTOs;
using PortfolioApi.Models;

namespace PortfolioApi.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Username == request.Username);
        
        if (user == null)
        {
            return new LoginResponse { Success = false, Message = "Invalid credentials" };
        }
        
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResponse { Success = false, Message = "Invalid credentials" };
        }
        
        var token = GenerateJwtToken(user);
        
        return new LoginResponse { Success = true, Message = "Login successful", Token = token };
    }
    
    public async Task<bool> CreateDefaultAdminAsync(string username, string password)
    {
        // Check if THIS specific admin exists (not just any admin)
        if (await _context.AdminUsers.AnyAsync(u => u.Username == username))
            return false;
        
        var admin = new AdminUser
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        
        _context.AdminUsers.Add(admin);
        await _context.SaveChangesAsync();
        return true;
    }
    
    private string GenerateJwtToken(AdminUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"] ?? "your-256-bit-secret-key-here-minimum-32-chars"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "PortfolioApi",
            audience: _configuration["Jwt:Audience"] ?? "PortfolioApp",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
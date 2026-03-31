using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioApi.Application.Features.Authentication.Commands;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<AdminUser> _userManager;
    private readonly SignInManager<AdminUser> _signInManager;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        UserManager<AdminUser> userManager,
        SignInManager<AdminUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Request.Username);
        
        if (user == null)
        {
            return new LoginResponse { Success = false, Message = "Invalid credentials" };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Request.Password, false);
        
        if (!result.Succeeded)
        {
            return new LoginResponse { Success = false, Message = "Invalid credentials" };
        }

        var token = GenerateJwtToken(user);
        
        return new LoginResponse { Success = true, Message = "Login successful", Token = token };
    }

    private string GenerateJwtToken(AdminUser user)
    {
        var jwtSecret = _configuration["Jwt:Secret"] ?? "menomo-portfolio-api-strong-secret-key";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
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

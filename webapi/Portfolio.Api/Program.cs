using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (isDevelopment && (connectionString == null || connectionString.Contains("your-server")))
    {
        options.UseSqlite("Data Source=portfolio.db");
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Identity
builder.Services.AddIdentity<AdminUser, IdentityRole<int>>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(PortfolioApi.Application.DTOs.LoginRequest).Assembly);
});

// Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "menomo-portfolio-api-strong-secret-key";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "PortfolioApi",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "PortfolioApp",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://your-portfolio.vercel.app",
            "https://*.vercel.app"
        )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default admin user and initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
    
    dbContext.Database.EnsureCreated();
    
    var adminUsername = builder.Configuration["Admin:Username"] ?? "Menomo";
    var adminPassword = builder.Configuration["Admin:Password"] ?? "Menomo@123";
    
    if (await userManager.FindByNameAsync(adminUsername) == null)
    {
        var admin = new AdminUser { UserName = adminUsername, Email = "admin@portfolio.com" };
        await userManager.CreateAsync(admin, adminPassword);
    }

    var seedService = new SeedService(dbContext);
    await seedService.SeedInitialDataAsync();
}

app.Run();
using System.Text;
using AspNetCoreRateLimit;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;

using PortfolioApi.Infrastructure.Services;
using PortfolioApi.Infrastructure.Services.Models;
using PortfolioApi.Application.Interfaces;
using System.Reflection;
using Serilog;

// Configure Serilog before builder
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/portfolio-.log", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Portfolio API...");
    
    var builder = WebApplication.CreateBuilder(args);
    
    // Use Serilog for logging
    builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Use SQLite for development when no connection string provided
    if (isDevelopment && (string.IsNullOrEmpty(connectionString) || connectionString?.Contains(".db") == true))
    {
        options.UseSqlite(connectionString ?? "Data Source=portfolio.db");
    }
    // Use PostgreSQL for production (Render) or when connection string indicates PostgreSQL
    else if (!string.IsNullOrEmpty(connectionString) && 
             (connectionString.Contains("Host=") || connectionString.Contains("Database=") || connectionString.Contains("Username=")))
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(3);
        });
    }
    // Fallback to SQL Server for other cases
    else if (!string.IsNullOrEmpty(connectionString))
    {
        options.UseSqlServer(connectionString);
    }
    // Default to SQLite if nothing else works
    else
    {
        options.UseSqlite("Data Source=portfolio.db");
    }
});

// Register IApplicationDbContext
builder.Services.AddScoped<PortfolioApi.Application.Interfaces.IApplicationDbContext>(provider => 
    provider.GetRequiredService<AppDbContext>());

// Identity
builder.Services.AddIdentity<AdminUser, IdentityRole<int>>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 6;
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(PortfolioApi.Application.DTOs.LoginRequest).Assembly);
});

// FluentValidation - manual validation only (no auto-validation to control error responses)
builder.Services.AddValidatorsFromAssemblyContaining<PortfolioApi.Api.Validators.LoginRequestValidator>();

// HttpContextAccessor for password change
builder.Services.AddHttpContextAccessor();

// Email Service Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailService>(sp => {
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailSettings>>().Value;
    return new EmailService(settings);
});

// Cloudinary Service for file uploads
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Authentication - JWT Secret must be configured via environment variables
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT Secret must be configured via environment variable 'Jwt__Secret'");

if (jwtSecret.Length < 32)
{
    throw new InvalidOperationException("JWT Secret must be at least 32 characters long");
}

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
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:4200" };
        
        policy.WithOrigins(allowedOrigins)
            .WithHeaders("Content-Type", "Authorization")
            .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
            .AllowCredentials();
    });
});

// Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/auth/login",
            Limit = 5,
            Period = "1m"
        }
    };
});
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseIpRateLimiting();
app.UseHttpsRedirection();

// Global Exception Handler Middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception occurred");
        
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        var errorResponse = new
        {
            Success = false,
            Message = app.Environment.IsDevelopment() 
                ? $"Internal server error: {ex.Message}" 
                : "An internal server error occurred"
        };
        
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
});

app.UseCors("AllowFrontend");

app.UseStaticFiles();

// Security Headers Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: blob:; font-src 'self'; connect-src 'self'; media-src 'self'; object-src 'none'; frame-ancestors 'none';");
    await next();
});

// Add request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

// Analytics middleware (before auth to capture all traffic)

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default admin user and initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
    
    // Apply migrations
    dbContext.Database.Migrate();
    
    var adminUsername = builder.Configuration["Admin:Username"]
        ?? throw new InvalidOperationException("Admin Username must be configured via environment variable 'Admin__Username'");
    var adminPassword = builder.Configuration["Admin:Password"]
        ?? throw new InvalidOperationException("Admin Password must be configured via environment variable 'Admin__Password'");
    
    if (await userManager.FindByNameAsync(adminUsername) == null)
    {
        var admin = new AdminUser { UserName = adminUsername, Email = "meno.mo.dev@gmail.com" };
        var result = await userManager.CreateAsync(admin, adminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    var seedService = new SeedService(dbContext);
    await seedService.SeedInitialDataAsync();
}

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

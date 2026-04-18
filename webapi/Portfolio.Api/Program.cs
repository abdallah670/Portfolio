using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    // Use SQLite for development or when connection string is SQLite format
    if (isDevelopment || connectionString?.Contains(".db") == true || string.IsNullOrEmpty(connectionString))
    {
        options.UseSqlite(connectionString ?? "Data Source=portfolio.db");
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Register IApplicationDbContext
builder.Services.AddScoped<PortfolioApi.Application.Interfaces.IApplicationDbContext>(provider => 
    provider.GetRequiredService<AppDbContext>());

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

// HttpContextAccessor for password change
builder.Services.AddHttpContextAccessor();

// Email Service Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailService>(sp => {
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailSettings>>().Value;
    return new EmailService(settings);
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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

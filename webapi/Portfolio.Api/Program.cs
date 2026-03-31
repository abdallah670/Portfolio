using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortfolioApi.Data;
using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database - Use SQLite for local, SQL Server for production
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

// Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PortfolioService>();
builder.Services.AddScoped<SeedService>();

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

// Serve static files for uploads
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = ""
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default admin user and initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Auto-migrate database
    dbContext.Database.EnsureCreated();
    
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    var adminUsername = builder.Configuration["Admin:Username"] ?? "Menomo";
    var adminPassword = builder.Configuration["Admin:Password"] ?? "Menomo@123";
    
    await authService.CreateDefaultAdminAsync(adminUsername, adminPassword);
    await seedService.SeedInitialDataAsync();
}

Console.WriteLine("API running at: http://localhost:5000");
Console.WriteLine("Swagger UI at: http://localhost:5000/swagger");

app.Run();
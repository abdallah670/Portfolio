using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Infrastructure.Middleware;

public class AnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AnalyticsMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private static readonly HashSet<string> ExcludedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/analytics",
        "/api/auth",
        "/health",
        "/swagger",
        "/favicon.ico",
        "/robots.txt",
        "/.well-known"
    };

    public AnalyticsMiddleware(
        RequestDelegate next,
        ILogger<AnalyticsMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        
        // Skip analytics for excluded paths and static files
        if (ShouldSkipTracking(path))
        {
            await _next(context);
            return;
        }

        // Get or create session
        var session = await GetOrCreateSessionAsync(context, dbContext);
        
        // Track page visit
        var pageVisit = await TrackPageVisitAsync(context, session, dbContext);
        
        // Continue request pipeline
        await _next(context);
        
        // Update session after response
        await UpdateSessionAsync(session, pageVisit, dbContext);
    }

    private bool ShouldSkipTracking(string path)
    {
        if (string.IsNullOrEmpty(path)) return true;
        
        // Skip static files
        if (path.StartsWith("/assets/") || 
            path.StartsWith("/uploads/") ||
            path.EndsWith(".js") || 
            path.EndsWith(".css") || 
            path.EndsWith(".png") || 
            path.EndsWith(".jpg") || 
            path.EndsWith(".svg") ||
            path.EndsWith(".ico"))
            return true;
        
        // Skip excluded paths
        return ExcludedPaths.Any(excluded => path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<VisitorSession> GetOrCreateSessionAsync(HttpContext context, AppDbContext dbContext)
    {
        var sessionId = GetSessionId(context);
        var ipAddress = GetIpAddress(context);
        var hashedIp = HashIpAddress(ipAddress);
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        
        // Try to find existing session
        var session = await dbContext.VisitorSessions
            .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        
        if (session != null)
        {
            // Update last activity
            session.EndedAt = DateTime.UtcNow;
            return session;
        }
        
        // Parse user agent for device info
        var (deviceType, browser, os) = ParseUserAgent(userAgent);
        
        // Get screen resolution from cookie if available
        var screenRes = context.Request.Cookies["screen_resolution"] ?? "Unknown";
        
        // Create new session
        session = new VisitorSession
        {
            SessionId = sessionId,
            HashedIpAddress = hashedIp,
            Country = await GetCountryAsync(ipAddress),
            City = "Unknown", // Would need geolocation service
            DeviceType = deviceType,
            Browser = browser,
            OperatingSystem = os,
            ScreenResolution = screenRes,
            StartedAt = DateTime.UtcNow,
            IsBounced = true,
            TotalPageViews = 0
        };
        
        dbContext.VisitorSessions.Add(session);
        await dbContext.SaveChangesAsync();
        
        // Set session cookie
        context.Response.Cookies.Append("visitor_session_id", sessionId, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddMinutes(GetSessionTimeout()),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps
        });
        
        return session;
    }

    private async Task<PageVisit> TrackPageVisitAsync(HttpContext context, VisitorSession session, AppDbContext dbContext)
    {
        var path = context.Request.Path.Value ?? "/";
        var referrer = context.Request.Headers["Referer"].ToString();
        
        // Extract project ID from path if visiting a project
        int? projectId = null;
        var projectMatch = Regex.Match(path, @"/projects?/(\d+)", RegexOptions.IgnoreCase);
        if (projectMatch.Success)
        {
            projectId = int.Parse(projectMatch.Groups[1].Value);
            
            // Increment project views
            await IncrementProjectViewsAsync(projectId.Value, dbContext);
        }
        
        var pageVisit = new PageVisit
        {
            SessionId = session.SessionId,
            Path = path,
            ProjectId = projectId,
            VisitedAt = DateTime.UtcNow,
            Referrer = referrer ?? string.Empty
        };
        
        dbContext.PageVisits.Add(pageVisit);
        await dbContext.SaveChangesAsync();
        
        return pageVisit;
    }

    private async Task IncrementProjectViewsAsync(int projectId, AppDbContext dbContext)
    {
        try
        {
            var project = await dbContext.Projects.FindAsync(projectId);
            if (project != null)
            {
                project.ViewsCount++;
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to increment project views for project {ProjectId}", projectId);
        }
    }

    private async Task UpdateSessionAsync(VisitorSession session, PageVisit pageVisit, AppDbContext dbContext)
    {
        session.TotalPageViews = await dbContext.PageVisits
            .CountAsync(v => v.SessionId == session.SessionId);
        
        // Mark as not bounced if more than 1 page view
        session.IsBounced = session.TotalPageViews <= 1;
        session.EndedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
    }

    private string GetSessionId(HttpContext context)
    {
        // Try to get existing session from cookie
        var existingSession = context.Request.Cookies["visitor_session_id"];
        if (!string.IsNullOrEmpty(existingSession))
        {
            return existingSession;
        }
        
        // Generate new session ID
        return Guid.NewGuid().ToString("N");
    }

    private string GetIpAddress(HttpContext context)
    {
        // Try X-Forwarded-For header first (for proxies)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',');
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }
        
        // Fall back to remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private string HashIpAddress(string ipAddress)
    {
        var salt = _configuration["Analytics:IpHashSalt"] ?? "default-salt-change-in-production";
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(ipAddress + salt);
        var hash = sha256.ComputeHash(combined);
        return Convert.ToHexString(hash);
    }

    private (string DeviceType, string Browser, string OS) ParseUserAgent(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return ("Desktop", "Unknown", "Unknown");
        
        var ua = userAgent.ToLower();
        
        // Device Type
        string deviceType;
        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            deviceType = "Mobile";
        else if (ua.Contains("tablet") || ua.Contains("ipad"))
            deviceType = "Tablet";
        else
            deviceType = "Desktop";
        
        // Browser
        string browser;
        if (ua.Contains("chrome")) browser = "Chrome";
        else if (ua.Contains("firefox")) browser = "Firefox";
        else if (ua.Contains("safari") && !ua.Contains("chrome")) browser = "Safari";
        else if (ua.Contains("edge")) browser = "Edge";
        else browser = "Other";
        
        // Operating System
        string os;
        if (ua.Contains("windows")) os = "Windows";
        else if (ua.Contains("macintosh") || ua.Contains("mac os")) os = "macOS";
        else if (ua.Contains("linux")) os = "Linux";
        else if (ua.Contains("android")) os = "Android";
        else if (ua.Contains("iphone") || ua.Contains("ipad")) os = "iOS";
        else os = "Unknown";
        
        return (deviceType, browser, os);
    }

    private async Task<string> GetCountryAsync(string ipAddress)
    {
        // Simplified geolocation - in production, use a service like MaxMind or IP-API
        // For now, return a placeholder
        return "Unknown";
    }

    private int GetSessionTimeout()
    {
        return _configuration.GetValue<int>("Analytics:SessionTimeoutMinutes", 30);
    }
}

// Extension method for easy registration
public static class AnalyticsMiddlewareExtensions
{
    public static IApplicationBuilder UseAnalytics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AnalyticsMiddleware>();
    }
}

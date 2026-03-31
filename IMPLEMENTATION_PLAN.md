# Portfolio Project: Phases 3-5 Implementation Plan

## Architecture Overview
- **Backend**: Clean Architecture (.NET) with Domain, Application, Infrastructure, API layers
- **Frontend**: Angular with standalone components
- **Database**: EF Core with SQLite (dev) / SQL Server (prod)
- **Pattern**: CQRS with MediatR

---

## 📊 Phase 3: Native Web Analytics Engine

### 3.1 Domain Layer - New Entities

#### VisitorSession.cs
Tracks unique visitor sessions with GDPR-compliant IP hashing.

```csharp
public class VisitorSession
{
    public int Id { get; set; }
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string HashedIpAddress { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // Mobile/Desktop/Tablet
    public string Browser { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string ScreenResolution { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public bool IsBounced { get; set; } = true;
    public int TotalPageViews { get; set; } = 0;
    
    public ICollection<PageVisit> PageVisits { get; set; } = new List<PageVisit>();
}
```

#### PageVisit.cs
Tracks individual page views linked to sessions.

```csharp
public class PageVisit
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    public int TimeOnPage { get; set; } = 0; // Seconds
    public string Referrer { get; set; } = string.Empty;
    
    public VisitorSession Session { get; set; } = null!;
}
```

### 3.2 Infrastructure Layer

#### AnalyticsMiddleware.cs
Intercepts requests to capture analytics data.

**Features:**
- Extracts and hashes IP address (SHA256 with salt for GDPR compliance)
- Parses User-Agent for device/browser/OS detection
- Session tracking via cookie (`visitor_session_id`)
- Geolocation (simplified or via external API)
- Bounce rate detection (single page view = bounce)

**IP Hashing (GDPR Compliant):**
```csharp
private string HashIpAddress(string ipAddress, string salt)
{
    using var sha256 = SHA256.Create();
    var combined = Encoding.UTF8.GetBytes(ipAddress + salt);
    var hash = sha256.ComputeHash(combined);
    return Convert.ToHexString(hash);
}
```

#### AppDbContext Updates
```csharp
public DbSet<VisitorSession> VisitorSessions { get; set; }
public DbSet<PageVisit> PageVisits { get; set; }
```

### 3.3 Application Layer

#### DTOs (AnalyticsDTOs.cs)

```csharp
public class TrafficTrendsDto
{
    public DateTime Date { get; set; }
    public int Visitors { get; set; }
    public int PageViews { get; set; }
}

public class ProjectViewsDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int Views { get; set; }
    public double Percentage { get; set; }
}

public class GeoLocationDto
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int VisitorCount { get; set; }
    public double Percentage { get; set; }
}

public class DeviceBreakdownDto
{
    public string DeviceType { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class DashboardAnalyticsDto
{
    public int TotalVisitors { get; set; }
    public int TotalPageViews { get; set; }
    public double BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<TrafficTrendsDto> TrafficTrends { get; set; } = new();
    public List<ProjectViewsDto> TopProjects { get; set; } = new();
    public List<GeoLocationDto> TopLocations { get; set; } = new();
    public List<DeviceBreakdownDto> DeviceBreakdown { get; set; } = new();
}
```

#### Queries

```csharp
// GetTrafficTrendsQuery.cs
public record GetTrafficTrendsQuery(int Days) : IRequest<List<TrafficTrendsDto>>;

// GetProjectViewsQuery.cs
public record GetProjectViewsQuery(int Days) : IRequest<List<ProjectViewsDto>>;

// GetGeoLocationsQuery.cs
public record GetGeoLocationsQuery(int Days) : IRequest<List<GeoLocationDto>>;

// GetDeviceBreakdownQuery.cs
public record GetDeviceBreakdownQuery(int Days) : IRequest<List<DeviceBreakdownDto>>;

// GetAnalyticsDashboardQuery.cs
public record GetAnalyticsDashboardQuery(int Days) : IRequest<DashboardAnalyticsDto>;
```

### 3.4 API Layer

#### AnalyticsController.cs

```csharp
[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly ISender _mediator;
    
    public AnalyticsController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    // GET /api/analytics/dashboard?days=30
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] int days = 30)
    {
        var data = await _mediator.Send(new GetAnalyticsDashboardQuery(days));
        return Ok(data);
    }
    
    // GET /api/analytics/traffic-trends?days=30
    [HttpGet("traffic-trends")]
    public async Task<IActionResult> GetTrafficTrends([FromQuery] int days = 30)
    {
        var data = await _mediator.Send(new GetTrafficTrendsQuery(days));
        return Ok(data);
    }
    
    // GET /api/analytics/project-views?days=30
    [HttpGet("project-views")]
    public async Task<IActionResult> GetProjectViews([FromQuery] int days = 30)
    {
        var data = await _mediator.Send(new GetProjectViewsQuery(days));
        return Ok(data);
    }
    
    // GET /api/analytics/geography?days=30
    [HttpGet("geography")]
    public async Task<IActionResult> GetGeography([FromQuery] int days = 30)
    {
        var data = await _mediator.Send(new GetGeoLocationsQuery(days));
        return Ok(data);
    }
    
    // GET /api/analytics/devices?days=30
    [HttpGet("devices")]
    public async Task<IActionResult> GetDevices([FromQuery] int days = 30)
    {
        var data = await _mediator.Send(new GetDeviceBreakdownQuery(days));
        return Ok(data);
    }
}
```

### 3.5 Frontend Integration

#### AnalyticsService (New)

```typescript
@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private readonly API_URL = 'http://localhost:5000/api/analytics';
  
  constructor(private http: HttpClient) {}
  
  getDashboard(days: number = 30): Observable<DashboardAnalytics> {
    return this.http.get<DashboardAnalytics>(`${this.API_URL}/dashboard?days=${days}`);
  }
  
  getTrafficTrends(days: number = 30): Observable<TrafficTrend[]> {
    return this.http.get<TrafficTrend[]>(`${this.API_URL}/traffic-trends?days=${days}`);
  }
  
  getProjectViews(days: number = 30): Observable<ProjectView[]> {
    return this.http.get<ProjectView[]>(`${this.API_URL}/project-views?days=${days}`);
  }
  
  getGeography(days: number = 30): Observable<GeoLocation[]> {
    return this.http.get<GeoLocation[]>(`${this.API_URL}/geography?days=${days}`);
  }
  
  getDevices(days: number = 30): Observable<DeviceBreakdown[]> {
    return this.http.get<DeviceBreakdown[]>(`${this.API_URL}/devices?days=${days}`);
  }
}
```

#### AnalyticsComponent Updates
- Replace mock data with real API calls
- Dynamic timeframe selection (30/90/365 days)
- Real-time charts using fetched data
- Geographic markers from API

---

## ⚙️ Phase 4: Application System Settings

### 4.1 Domain Layer

#### SystemSetting.cs
Dynamic key-value store for application preferences.

```csharp
public class SystemSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string DataType { get; set; } = "string"; // string/int/bool/json
    public string Category { get; set; } = "general"; // ui/security/notifications
    public string Description { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    
    // Helper methods for type conversion
    public T? GetValue<T>()
    {
        if (string.IsNullOrEmpty(Value)) return default;
        
        return DataType.ToLower() switch
        {
            "bool" => (T)(object)bool.Parse(Value),
            "int" => (T)(object)int.Parse(Value),
            "json" => JsonSerializer.Deserialize<T>(Value),
            _ => (T)(object)Value
        };
    }
    
    public void SetValue<T>(T value)
    {
        Value = value switch
        {
            bool b => b.ToString().ToLower(),
            int i => i.ToString(),
            _ when typeof(T) == typeof(object) || typeof(T).IsClass => JsonSerializer.Serialize(value),
            _ => value?.ToString() ?? string.Empty
        };
        DataType = value switch
        {
            bool => "bool",
            int => "int",
            _ when typeof(T) == typeof(object) || typeof(T).IsClass => "json",
            _ => "string"
        };
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### 4.2 Application Layer

#### Commands

```csharp
// UpdateSystemSettingCommand.cs
public record UpdateSystemSettingCommand(string Key, string Value, string DataType) 
    : IRequest<bool>;

// UpdatePasswordCommand.cs
public record UpdatePasswordCommand(string CurrentPassword, string NewPassword) 
    : IRequest<PasswordUpdateResult>;

public class PasswordUpdateResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}
```

#### Queries

```csharp
// GetSystemSettingsQuery.cs
public record GetSystemSettingsQuery(string? Category = null) 
    : IRequest<List<SystemSetting>>;

// GetSystemSettingByKeyQuery.cs
public record GetSystemSettingByKeyQuery(string Key) 
    : IRequest<SystemSetting?>;
```

### 4.3 API Layer

#### SettingsController.cs

```csharp
[ApiController]
[Route("api/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly ISender _mediator;
    
    public SettingsController(ISender mediator)
    {
        _mediator = mediator;
    }
    
    // GET /api/settings
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var settings = await _mediator.Send(new GetSystemSettingsQuery(category));
        return Ok(settings);
    }
    
    // GET /api/settings/{key}
    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey(string key)
    {
        var setting = await _mediator.Send(new GetSystemSettingByKeyQuery(key));
        return setting != null ? Ok(setting) : NotFound();
    }
    
    // PUT /api/settings
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSettingRequest request)
    {
        var result = await _mediator.Send(new UpdateSystemSettingCommand(
            request.Key, request.Value, request.DataType));
        return result ? Ok() : BadRequest();
    }
}

public class UpdateSettingRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
}
```

#### AuthController Updates

```csharp
// PUT /api/auth/password
[HttpPut("password")]
[Authorize]
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
{
    var result = await _mediator.Send(new UpdatePasswordCommand(
        request.CurrentPassword, request.NewPassword));
    
    if (!result.Success)
        return BadRequest(new { errors = result.Errors });
    
    return Ok(new { message = "Password updated successfully" });
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
```

### 4.4 Frontend Integration

#### Models Update

```typescript
// portfolio.models.ts
export interface SystemSetting {
  id: number;
  key: string;
  value: string;
  dataType: 'string' | 'int' | 'bool' | 'json';
  category: string;
  description: string;
  updatedAt: string;
  updatedBy?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
```

#### ApiService Updates

```typescript
// Settings endpoints
getSettings(category?: string): Observable<SystemSetting[]> {
  let params = new HttpParams();
  if (category) params = params.set('category', category);
  return this.http.get<SystemSetting[]>(`${this.API_URL}/settings`, { params });
}

updateSetting(key: string, value: string, dataType: string = 'string'): Observable<void> {
  return this.http.put<void>(`${this.API_URL}/settings`, { key, value, dataType });
}

updatePassword(currentPassword: string, newPassword: string): Observable<{ message: string }> {
  return this.http.put<{ message: string }>(`${this.API_URL}/auth/password`, { 
    currentPassword, newPassword 
  });
}
```

#### SettingsComponent Updates
- Wire up password change form with validation
- Load/save settings from API
- Success/error toast notifications

---

## 🚀 Phase 5: Admin Projects & Verification

### 5.1 Domain Layer Updates

#### Project.cs - New Properties

```csharp
public class Project
{
    // ... existing properties
    
    public bool IsPublished { get; set; } = true;
    public int ViewsCount { get; set; } = 0;
}
```

### 5.2 Application Layer Updates

#### Commands

```csharp
// PublishProjectCommand.cs
public record PublishProjectCommand(int ProjectId) : IRequest<bool>;

// UnpublishProjectCommand.cs
public record UnpublishProjectCommand(int ProjectId) : IRequest<bool>;

// IncrementProjectViewsCommand.cs (called by middleware)
public record IncrementProjectViewsCommand(int ProjectId) : IRequest;
```

### 5.3 API Layer Updates

#### PortfolioController Updates

```csharp
// PUT /api/portfolio/projects/{id}/publish
[HttpPut("projects/{id}/publish")]
[Authorize]
public async Task<IActionResult> PublishProject(int id)
{
    var result = await _mediator.Send(new PublishProjectCommand(id));
    return result ? Ok() : NotFound();
}

// PUT /api/portfolio/projects/{id}/unpublish
[HttpPut("projects/{id}/unpublish")]
[Authorize]
public async Task<IActionResult> UnpublishProject(int id)
{
    var result = await _mediator.Send(new UnpublishProjectCommand(id));
    return result ? Ok() : NotFound();
}

// Public endpoint - only return published projects
[HttpGet("projects")]
[AllowAnonymous]
public async Task<IActionResult> GetPublicProjects()
{
    var projects = await _context.Projects
        .Where(p => p.IsPublished)
        .ToListAsync();
    return Ok(projects);
}
```

### 5.4 Frontend Integration

#### Models Update

```typescript
// portfolio.models.ts - Project interface
export interface Project {
  id: number;
  title: string;
  description: string;
  // ... other existing fields
  isPublished: boolean;
  viewsCount: number;
}
```

#### ApiService Updates

```typescript
// Project publish/unpublish
publishProject(id: number): Observable<void> {
  return this.http.put<void>(`${this.API_URL}/portfolio/projects/${id}/publish`, {});
}

unpublishProject(id: number): Observable<void> {
  return this.http.put<void>(`${this.API_URL}/portfolio/projects/${id}/unpublish`, {});
}
```

#### ProjectsComponent Updates
- Wire up Draft/Publish toggle buttons
- Display views count on project cards
- Refresh project list after status change

### 5.5 Database Migration

```bash
# Navigate to Infrastructure project
cd webapi/Portfolio.Infrastructure

# Create migration
dotnet ef migrations add Phase5 --startup-project ../Portfolio.Api

# Update database
dotnet ef database update --startup-project ../Portfolio.Api
```

---

## 📋 Implementation Checklist

### Phase 3: Analytics
- [ ] Create VisitorSession entity
- [ ] Create PageVisit entity
- [ ] Build AnalyticsMiddleware with GDPR-compliant IP hashing
- [ ] Update AppDbContext & IApplicationDbContext
- [ ] Create Analytics DTOs
- [ ] Create Analytics Queries and Handlers
- [ ] Create AnalyticsController
- [ ] Register AnalyticsMiddleware in Program.cs
- [ ] Create AnalyticsService (frontend)
- [ ] Update portfolio.models.ts with analytics interfaces
- [ ] Update AnalyticsComponent with real data

### Phase 4: Settings
- [ ] Create SystemSetting entity
- [ ] Update AppDbContext & IApplicationDbContext
- [ ] Create Settings DTOs
- [ ] Create Settings Commands and Handlers
- [ ] Create Settings Queries and Handlers
- [ ] Create SettingsController
- [ ] Add PUT /api/auth/password endpoint to AuthController
- [ ] Create UpdatePasswordCommand and Handler
- [ ] Update ApiService with settings methods
- [ ] Update portfolio.models.ts with SystemSetting interface
- [ ] Wire up SettingsComponent forms

### Phase 5: Projects
- [ ] Add IsPublished & ViewsCount to Project entity
- [ ] Create Publish/Unpublish commands
- [ ] Create IncrementProjectViewsCommand
- [ ] Update AnalyticsMiddleware to increment project views
- [ ] Add publish/unpublish endpoints to PortfolioController
- [ ] Update GetPublicProjects to filter by IsPublished
- [ ] Update Project model (frontend)
- [ ] Update ApiService with publish/unpublish methods
- [ ] Wire up ProjectsComponent toggle
- [ ] Generate EF Migration (Phase5)
- [ ] Test full application

---

## 🏗️ Key Design Decisions

1. **GDPR-Compliant IP Hashing**: Using SHA256 with a configurable salt to anonymize IPs while maintaining ability to identify unique sessions.

2. **Analytics Data Retention**: No automatic purge implemented - can be added later via settings.

3. **SystemSetting Type Safety**: Values stored as strings with DataType metadata, parsed by application layer.

4. **ViewsCount Implementation**: Simple counter on Project entity, incremented by AnalyticsMiddleware when project detail pages are visited.

5. **Middleware Ordering**: AnalyticsMiddleware placed early in pipeline (after CORS, before Auth) to capture all traffic including anonymous.

6. **Session Management**: 30-minute session timeout, tracked via cookie with sliding expiration.

7. **Bounce Rate Calculation**: Session is marked as bounced if only 1 page view occurs.

---

## 🔧 Configuration

Add to `appsettings.json`:
```json
{
  "Analytics": {
    "IpHashSalt": "your-random-salt-here",
    "SessionTimeoutMinutes": 30,
    "Enabled": true
  }
}
```

---

## 🚀 Getting Started

1. Toggle to Act Mode
2. Run implementation following the checklist
3. Generate migrations
4. Test the application
5. Verify all endpoints in Swagger UI

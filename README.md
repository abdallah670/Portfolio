# Portfolio API - Deployment Guide

## MonsterASP.net Deployment

### Step 1: Create MonsterASP.net Account

1. Go to https://www.monsterasp.net
2. Click "Try it for FREE"
3. Sign up (no credit card required)

### Step 2: Create Database

1. After login, go to Control Panel
2. Create a new SQL Server database:
   - Name: `PortfolioApi`
   - Note the connection details (Server, Database, Username, Password)

### Step 3: Configure Connection String

Update `appsettings.json` with your MonsterASP database info:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server.monsterasp.net;Database=PortfolioApi;User=your-username;Password=your-password;TrustServerCertificate=True"
  },
  "Jwt": {
    "Secret": "YOUR-SECRET-KEY-MINIMUM-32-CHARACTERS",
    "Issuer": "PortfolioApi",
    "Audience": "PortfolioApp"
  },
  "Admin": {
    "Username": "admin",
    "Password": "YourSecurePassword123"
  }
}
```

### Step 4: Deploy to MonsterASP

#### Option A: GitHub Deployment (Recommended)

1. Push your code to GitHub
2. In MonsterASP Control Panel:
   - Select "Deploy from GitHub"
   - Authorize MonsterASP to access your GitHub
   - Select the `webapi` folder as the project root
   - Configure build command: `dotnet publish -c Release`
   - Configure start command: `dotnet PortfolioApi.dll`

#### Option B: FTP Upload

1. Build locally:
   ```bash
   cd webapi
   dotnet publish -c Release
   ```

2. Upload contents of `webapi/bin/Release/net9.0/publish/` to MonsterASP via FTP

### Step 5: Environment Variables (MonsterASP Panel)

Set these in the MonsterASP control panel environment variables:

| Variable | Value |
|----------|-------|
| `ConnectionStrings__DefaultConnection` | Your connection string |
| `Jwt__Secret` | Your secret key |
| `Jwt__Issuer` | PortfolioApi |
| `Jwt__Audience` | PortfolioApp |
| `Admin__Username` | admin |
| `Admin__Password` | yourpassword |

### Step 6: Update CORS in Program.cs

Before deploying, update the CORS policy with your actual frontend URL:

```csharp
policy.WithOrigins(
    "http://localhost:4200",  // Local dev
    "https://your-frontend.vercel.app",  // Vercel URL
    "https://your-custom-domain.com"  // Your domain
)
```

---

## API Endpoints

### Public Endpoints (No Auth Required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/portfolio/config` | Get full portfolio configuration |
| GET | `/api/portfolio/skills` | Get all skills |
| GET | `/api/portfolio/projects` | Get all projects |

### Protected Endpoints (Require JWT Token)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login and get JWT token |
| PUT | `/api/portfolio/hero` | Update hero section |
| PUT | `/api/portfolio/about` | Update about section |
| POST | `/api/portfolio/projects` | Create project |
| PUT | `/api/portfolio/projects` | Update project |
| DELETE | `/api/portfolio/projects/{id}` | Delete project |
| POST | `/api/portfolio/journey` | Create journey item |
| PUT | `/api/portfolio/journey` | Update journey item |
| DELETE | `/api/portfolio/journey/{id}` | Delete journey item |
| PUT | `/api/portfolio/contact` | Update contact info |
| POST | `/api/portfolio/socials` | Create social link |
| PUT | `/api/portfolio/socials` | Update social link |
| DELETE | `/api/portfolio/socials/{id}` | Delete social link |
| POST | `/api/portfolio/skills/categories` | Create skill category |
| PUT | `/api/portfolio/skills/categories` | Update skill category |
| POST | `/api/portfolio/skills` | Create skill |
| DELETE | `/api/portfolio/skills/{id}` | Delete skill |

### Upload Endpoints (Require Auth)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/upload/project-image` | Upload project image |
| POST | `/api/upload/profile-image` | Upload profile image |

---

## Testing Locally

1. Update `appsettings.json` with your local SQL Server connection
2. Run migrations:
   ```bash
   cd webapi
   dotnet ef database update
   ```
3. Run the API:
   ```bash
   dotnet run
   ```
4. Access Swagger: https://localhost:5001/swagger

---

## Frontend Configuration

After deployment, update your Angular frontend `environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api.monsterasp.net',  // Your MonsterASP API URL
  // ... other settings
};
```

---

## Default Admin Credentials

- **Username**: admin (or custom from config)
- **Password**: admin123 (or custom from config)

**Important**: Change these in production!

---

## Project Structure

```
webapi/
├── Controllers/       # API endpoints
│   ├── AuthController.cs
│   ├── PortfolioController.cs
│   └── UploadController.cs
├── Data/              # Database context
│   └── AppDbContext.cs
├── DTOs/              # Data transfer objects
│   └── DTOs.cs
├── Models/            # Entity models
│   ├── AdminUser.cs
│   ├── Hero.cs
│   ├── About.cs
│   ├── Skill.cs
│   ├── Project.cs
│   ├── JourneyItem.cs
│   └── SocialContact.cs
├── Services/          # Business logic
│   ├── AuthService.cs
│   ├── PortfolioService.cs
│   └── SeedService.cs
├── wwwroot/           # Static files
│   └── uploads/       # Uploaded images
├── Program.cs         # App configuration
└── appsettings.json   # Configuration
```
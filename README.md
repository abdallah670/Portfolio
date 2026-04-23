# Portfolio Application

A full-stack portfolio website with admin panel, built with .NET 10 Web API and Angular.

## Tech Stack

**Backend:**
- .NET 10 Web API
- Entity Framework Core
- PostgreSQL (Supabase)
- JWT Authentication
- Cloudinary (image storage)
- Email (Gmail SMTP)

**Frontend:**
- Angular 19
- TypeScript
- SCSS
- Responsive design

**Deployment:**
- Backend: MonsterASP (free hosting)
- Frontend: Netlify (free hosting)
- Database: Supabase PostgreSQL

## Project Structure

```
Portfolio/
├── frontend/                 # Angular frontend
│   ├── src/app/
│   │   ├── components/      # Public site components
│   │   ├── admin/           # Admin panel
│   │   └── services/        # API services
│   └── dist/                # Build output
├── webapi/                  # .NET backend
│   ├── Portfolio.Api/       # Web API
│   ├── Portfolio.Application/
│   ├── Portfolio.Domain/
│   └── Portfolio.Infrastructure/
└── .github/workflows/       # CI/CD
```

## Local Development

### Prerequisites
- .NET 10 SDK
- Node.js 20+
- PostgreSQL (or use Supabase)

### Backend Setup

```bash
cd webapi/Portfolio.Api
dotnet restore
dotnet run
```

API runs at: `https://localhost:5001` or `http://localhost:5000`

### Frontend Setup

```bash
cd frontend
npm install
ng serve
```

Frontend runs at: `http://localhost:4200`

### Environment Variables

Create `webapi/Portfolio.Api/.env`:

```env
ConnectionStrings__DefaultConnection=your-postgresql-connection
Jwt__Secret=your-secret-key
Admin__Username=admin
Admin__Password=your-password
EmailSettings__SmtpHost=smtp.gmail.com
EmailSettings__SmtpUser=your-email@gmail.com
EmailSettings__SmtpPass=your-app-password
EmailSettings__FromEmail=your-email@gmail.com
Cloudinary__CloudName=your-cloud-name
Cloudinary__ApiKey=your-api-key
Cloudinary__ApiSecret=your-api-secret
AllowedOrigins=http://localhost:4200
ASPNETCORE_ENVIRONMENT=Development
```

## Deployment

### Backend (MonsterASP)

1. **Create MonsterASP Account:**
   - Go to [monsterasp.net](https://www.monsterasp.net/)
   - Sign up for free hosting
   - Create website, note your site ID (e.g., `site64972`)

2. **Get WebDeploy Credentials:**
   - In MonsterASP control panel, go to Deploy → WebDeploy access
   - Note these values:
     - Website name: `siteXXXX`
     - Server: `https://siteXXXX.siteasp.net:8172`
     - Username: `siteXXXX`
     - Password: `********`

3. **Add GitHub Secrets:**
   - Go to GitHub repo → Settings → Secrets and variables → Actions
   - Add these secrets:
     - `WEBSITE_NAME` = your site name
     - `SERVER_COMPUTER_NAME` = your server URL
     - `SERVER_USERNAME` = your username
     - `SERVER_PASSWORD` = your password

4. **Add Environment Variables in MonsterASP:**
   - Control panel → Environment variables
   - Add all variables from your `.env` file

5. **Deploy:**
   - Push to `main` branch
   - GitHub Actions automatically deploys
   - Check Actions tab for status

**Your API URL:** `https://[site-name].runasp.net`

### Frontend (Netlify)

1. **Build:**
   ```bash
   cd frontend
   npm run build
   ```

2. **Deploy:**
   - Go to [netlify.com](https://netlify.com)
   - Drag and drop `dist/frontend/browser` folder
   - Or connect GitHub repo for auto-deploy

3. **Update API URL:**
   - Edit `frontend/src/environments/environment.prod.ts`
   - Set `apiUrl` to your MonsterASP URL + `/api`
   - Rebuild and redeploy

**Your Frontend URL:** `https://[site-name].netlify.app`

## Features

- **Public Site:**
  - Hero section with animated text
  - Projects showcase
  - Skills display
  - About section
  - Contact form with email
  - CV download

- **Admin Panel:**
  - JWT authentication
  - Manage projects (CRUD)
  - Upload images to Cloudinary
  - Manage contact messages
  - Update hero/about sections
  - Upload CV

## API Endpoints

| Endpoint | Description |
|----------|-------------|
| `POST /api/auth/login` | Admin login |
| `GET /api/portfolio` | Get all portfolio data |
| `GET /api/projects` | Get all projects |
| `POST /api/projects` | Create project (admin) |
| `PUT /api/projects/{id}` | Update project (admin) |
| `DELETE /api/projects/{id}` | Delete project (admin) |
| `POST /api/upload/image` | Upload image (admin) |
| `POST /api/upload/cv` | Upload CV (admin) |
| `POST /api/messages` | Send contact message |
| `GET /api/messages` | Get messages (admin) |

## License

MIT

## Author

Abdallah Mohammed

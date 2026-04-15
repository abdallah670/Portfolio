# Frontend ↔ Backend Connection Plan

> **Backend**: Running at `http://localhost:5000`  
> **Frontend**: Angular standalone app at `c:\Meno\Projects\Portfolio\frontend`  
> **Status**: Backend verified working. All public components use hardcoded data. This plan wires everything up.

---

## API Reference

### `GET /api/portfolio/config` — Public, no auth needed
```json
{
  "hero": {
    "name": "Abdullah Mohammed",
    "headlineTop": "Hi, I'm",
    "headlineMain": "Abdullah Mohammed",
    "availabilityLabel": "Available for Opportunities",
    "subtitle": "Full-Stack .NET Developer",
    "heroIntro": "Backend-focused .NET developer...",
    "ctaPrimaryLabel": "View My Work",
    "ctaPrimaryHref": "/projects",
    "ctaSecondaryLabel": "Get In Touch",
    "ctaSecondaryHref": "/contact",
    "profileImage": "uploads/profile-image/Meno.png",
    "stats": [{ "label": "Projects", "value": "5+" }]
  },
  "about": {
    "kicker": "Get to Know Me",
    "title": "About Me",
    "subtitle": "...",
    "funFact": "...",
    "cards": [{ "title": ".NET Developer", "subtitle": "C#, ADO.NET" }],
    "achievements": ["Built online coaching system..."],
    "values": [{ "title": "Clean Architecture", "description": "..." }]
  },
  "skills": [
    { "title": "Backend Development", "color": "emerald",
      "skills": [{ "name": "C#", "level": 85 }] }
  ],
  "featuredProjects": [
    { "id": 1, "title": "...", "stack": "[\"ASP.NET Core\",\"SQL\"]",
      "image": "...", "liveUrl": "...", "githubUrl": "...", "isFeatured": true }
  ],
  "moreProjects": [...],
  "journey": [{ "id": 1, "title": "...", "period": "...", "org": "...", "description": "..." }],
  "socials": [{ "label": "GitHub", "href": "...", "icon": "github" }],
  "contact": { "email": "...", "whatsApp": "...", "phone": "...", "location": "..." }
}
```

### Auth Endpoints
| Method | URL | Auth? | Body |
|---|---|---|---|
| POST | `/api/auth/login` | No | `{ username, password }` |
| PUT | `/api/auth/password` | Yes | `{ currentPassword, newPassword }` |

### Portfolio Endpoints
| Method | URL | Auth? | Notes |
|---|---|---|---|
| GET | `/api/portfolio/config` | No | Full public config |
| GET | `/api/portfolio/skills` | No | Skill categories |
| GET | `/api/portfolio/projects` | No | Published only |
| GET | `/api/portfolio/admin/projects` | Yes | All projects incl. drafts |
| POST | `/api/portfolio/projects` | Yes | Create project |
| PUT | `/api/portfolio/projects` | Yes | Update project |
| DELETE | `/api/portfolio/projects/{id}` | Yes | Delete project |
| PUT | `/api/portfolio/projects/{id}/publish` | Yes | Publish |
| PUT | `/api/portfolio/projects/{id}/unpublish` | Yes | Unpublish |
| PUT | `/api/portfolio/hero` | Yes | Update hero |
| GET | `/api/portfolio/dashboard-stats` | Yes | Dashboard metrics |

### Messages Endpoints
| Method | URL | Auth? |
|---|---|---|
| POST | `/api/messages` | No (contact form) |
| GET | `/api/messages?page=1&pageSize=20&isRead=false` | Yes |
| GET | `/api/messages/{id}` | Yes |
| PUT | `/api/messages/{id}/read` | Yes |
| DELETE | `/api/messages/{id}` | Yes |
| GET | `/api/messages/unread-count` | Yes |

---

## Field Mismatch Reference

These are the exact mismatches found between frontend code and backend responses:

| Component | Frontend uses | Backend returns | Fix |
|---|---|---|---|
| skills.html | `category.name` | `category.title` | Change to `title` |
| skills.html | `skill.label` | `skill.name` | Change to `name` |
| projects.html | `project.tags` | `project.stack` (JSON string) | Parse JSON |
| projects.html | `project.link` | `project.liveUrl` | Change field |
| about.html | `val.desc` | `val.description` | Change field |
| models.ts | `JourneyItem.organization` | `org` | Rename |
| models.ts | `Hero.intro` | `heroIntro` | Rename |
| models.ts | `Contact.whatsapp` | `whatsApp` | Fix casing |
| dashboard | `stats.totalMessages` | not returned | Add to backend |

---

## Step-by-Step Execution

---

### ⬜ STEP 1 — Fix `portfolio.models.ts`
**File:** `frontend/src/app/core/models/portfolio.models.ts`  
**Status:** ⬜ PENDING

Add `PortfolioConfig` interface and fix all field names to match API exactly:
- `HeroConfig` with `heroIntro` (not `intro`)
- `AboutConfig` with `values[].description` (not `desc`)
- `SkillCategoryConfig` with `title` (not `name`) and `skills[].name` (not `label`)
- `ProjectConfig` with `liveUrl` (not `link`), `stack: string` (raw JSON)
- `JourneyItemConfig` with `org` (not `organization`)
- `ContactConfig` with `whatsApp` (capital A)

**ADD: PagedResult Interface (missing from original plan)**
```typescript
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
```

---

### ⬜ STEP 2 — Create Auth Guard
**File:** `frontend/src/app/core/guards/auth.guard.ts`  
**Status:** ⬜ PENDING

```typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isAuthenticated()) return true;
  router.navigate(['/login']);
  return false;
};
```

---

### STEP 3 — Create HTTP Interceptor for JWT Tokens ⭐ NEW
**File:** `frontend/src/app/core/interceptors/auth.interceptor.ts`  
**Status:** ⬜

**CRITICAL MISSING PIECE**: This interceptor attaches JWT tokens to authenticated requests and handles 401 errors.

```typescript
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptorFn: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  const token = authService.getToken();
  
  // Clone request and add auth token if available
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Token expired or invalid - logout and redirect
        authService.logout();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
```

**Register in `app.config.ts`:**
```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authInterceptorFn } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptorFn]))
  ]
};
```

---

### STEP 4 — Protect Admin Routes
**File:** `frontend/src/app/admin/admin.routes.ts`

```typescript
import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './admin-layout.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { authGuard } from '../core/guards/auth.guard';  // ADD

export const adminRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [authGuard],  // ADD
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'projects', loadComponent: () => import('./pages/projects/projects.component').then(m => m.ProjectsComponent) },
      { path: 'messages', loadComponent: () => import('./pages/messages/messages.component').then(m => m.MessagesComponent) },
      { path: 'skills', loadComponent: () => import('./pages/skills/skills.component').then(m => m.SkillsComponent) },
      { path: 'settings', loadComponent: () => import('./pages/settings/settings.component').then(m => m.SettingsComponent) },
      { path: 'analytics', loadComponent: () => import('./pages/analytics/analytics.component').then(m => m.AnalyticsComponent) }
    ]
  }
];
```

---

### STEP 5 — Wire Home Component (Fetch Config Once)
**File:** `frontend/src/app/public/pages/home/home.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { PortfolioConfig } from '../../../core/models/portfolio.models';
// Import child components
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { HeroComponent } from './components/hero/hero.component';
import { AboutComponent } from './components/about/about.component';
import { SkillsComponent } from './components/skills/skills.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, NavbarComponent, HeroComponent, AboutComponent,
            SkillsComponent, ProjectsComponent, FooterComponent],
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  config: PortfolioConfig | null = null;
  loading = true;
  error = '';

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getPortfolioConfig().subscribe({
      next: (data) => { 
        this.config = data; 
        this.loading = false; 
      },
      error: (err) => { 
        this.error = 'Failed to load portfolio data. Please try again later.';
        this.loading = false; 
      }
    });
  }
}
```

**home.component.html** — pass config as inputs:
```html
@if (loading) { 
  <div class="loading-container">
    <div class="spinner"></div>
    <p>Loading portfolio...</p>
  </div> 
}
@if (error) { 
  <div class="error-container">
    <p>{{ error }}</p>
    <button (click)="ngOnInit()">Retry</button>
  </div> 
}
@if (config) {
  <app-navbar></app-navbar>
  <app-home-hero [hero]="config.hero"></app-home-hero>
  <app-home-about [about]="config.about"></app-home-about>
  <app-home-skills [skills]="config.skills"></app-home-skills>
  <app-home-projects
    [featuredProjects]="config.featuredProjects"
    [moreProjects]="config.moreProjects">
  </app-home-projects>
  <app-footer [contact]="config.contact" [socials]="config.socials"></app-footer>
}
```

---

### STEP 6 — Wire Hero Component
**File:** `frontend/src/app/public/pages/home/components/hero/hero.component.ts`

```typescript
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeroConfig } from '../../../../../core/models/portfolio.models';

@Component({ 
  selector: 'app-home-hero', 
  standalone: true, 
  imports: [CommonModule],
  templateUrl: './hero.component.html', 
  styleUrls: ['./hero.component.scss'] 
})
export class HeroComponent {
  @Input() hero!: HeroConfig;

  get imageUrl(): string {
    if (!this.hero?.profileImage) return '';
    if (this.hero.profileImage.startsWith('http')) return this.hero.profileImage;
    return `http://localhost:5000/${this.hero.profileImage}`;
  }
}
```

**hero.component.html** — replace all static text:
```html
<section class="hero-section">
  <div class="container hero-grid">
    <div class="hero-content">
      <span class="hero-kicker">{{ hero?.availabilityLabel }}</span>
      <h1 class="hero-title">
        {{ hero?.headlineTop }}<br/>
        <span class="highlight-text">{{ hero?.headlineMain }}</span>
      </h1>
      <p class="hero-desc">{{ hero?.heroIntro }}</p>
      <div class="hero-actions">
        <a [href]="hero?.ctaPrimaryHref" class="btn btn-primary">{{ hero?.ctaPrimaryLabel }}</a>
        <a [href]="hero?.ctaSecondaryHref" class="btn btn-secondary">{{ hero?.ctaSecondaryLabel }}</a>
      </div>
      <div class="hero-stats">
        @for (stat of hero?.stats; track stat.label) {
          <div class="stat">
            <span class="stat-value">{{ stat.value }}</span>
            <span class="stat-label">{{ stat.label }}</span>
          </div>
        }
      </div>
    </div>
    <div class="hero-visual">
      <img [src]="imageUrl" [alt]="hero?.name" />
    </div>
  </div>
</section>
```

---

### STEP 7 — Wire About Component
**File:** `frontend/src/app/public/pages/home/components/about/about.component.ts`

```typescript
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AboutConfig } from '../../../../../core/models/portfolio.models';

@Component({ 
  selector: 'app-home-about', 
  standalone: true, 
  imports: [CommonModule],
  templateUrl: './about.component.html', 
  styleUrls: ['./about.component.scss'] 
})
export class AboutComponent {
  @Input() about!: AboutConfig;
}
```

**about.component.html** — critical fix is `val.description` (not `val.desc`):
```html
<!-- Cards loop — title & subtitle match API ✅ -->
@for (card of about?.cards; track card.title) {
  <div class="about-card">
    <div class="about-card-title">{{ card.title }}</div>
    <div class="about-card-subtitle">{{ card.subtitle }}</div>
  </div>
}

<!-- Achievements — plain strings ✅ -->
@for (achieve of about?.achievements; track achieve) {
  <div class="achievement-item">
    <span class="dot"></span>
    <p class="achievement-text">{{ achieve }}</p>
  </div>
}

<!-- Values — use description not desc ⚠️ -->
@for (val of about?.values; track val.title) {
  <div class="value-item">
    <span class="dot"></span>
    <div class="value-text">
      <strong>{{ val.title }}</strong> {{ val.description }}
    </div>
  </div>
}

<!-- Fun fact -->
<p class="fun-fact">
  <span class="fun-fact-label">Fun fact:</span> {{ about?.funFact }}
</p>
```

---

### STEP 8 — Wire Skills Component
**File:** `frontend/src/app/public/pages/home/components/skills/skills.component.ts`

```typescript
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkillCategoryConfig } from '../../../../../core/models/portfolio.models';

@Component({ 
  selector: 'app-home-skills', 
  standalone: true, 
  imports: [CommonModule],
  templateUrl: './skills.component.html', 
  styleUrls: ['./skills.component.scss'] 
})
export class SkillsComponent {
  @Input() skills: SkillCategoryConfig[] = [];
}
```

**skills.component.html** — fix field names (template was using wrong fields):
```html
<div class="skills-grid">
  @for (category of skills; track category.title) {      <!-- title not name -->
    <div class="skill-category-card">
      <div class="skill-category-head">
        <div class="skill-category-name">{{ category.title }}</div>
        <span class="skill-accent" [ngClass]="category.color"></span>
      </div>
      <div class="skill-list">
        @for (skill of category.skills; track skill.name) {  <!-- name not label -->
          <div class="skill-row">
            <div class="skill-label">{{ skill.name }}</div>
            <div class="skill-bar">
              <div class="skill-fill" [style.width.%]="skill.level"></div>
            </div>
            <div class="skill-level">{{ skill.level }}%</div>
          </div>
        }
      </div>
    </div>
  }
</div>
```

---

### STEP 9 — Wire Projects Component
**File:** `frontend/src/app/public/pages/home/components/projects/projects.component.ts`

```typescript
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProjectConfig } from '../../../../../core/models/portfolio.models';

@Component({ 
  selector: 'app-home-projects', 
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './projects.component.html', 
  styleUrls: ['./projects.component.scss'] 
})
export class ProjectsComponent {
  @Input() featuredProjects: ProjectConfig[] = [];
  @Input() moreProjects: ProjectConfig[] = [];

  get allProjects(): ProjectConfig[] {
    return [...this.featuredProjects, ...this.moreProjects];
  }

  getStack(project: ProjectConfig): string[] {
    try { return JSON.parse(project.stack); }
    catch { return []; }
  }

  getImageUrl(path: string | null | undefined): string {
    if (!path) return '';
    if (path.startsWith('http')) return path;
    return `http://localhost:5000/${path}`;
  }
}
```

**projects.component.html** — fix `tags`→parsed stack, `link`→`liveUrl`:
```html
<div class="projects-grid">
  @for (project of allProjects; track project.id) {
    <div class="project-card" [class.wide]="project.isFeatured">
      <div class="project-img-wrapper">
        <img [src]="getImageUrl(project.image)" [alt]="project.title" />
      </div>
      <div class="project-content">
        <div class="tags-container">
          @for (tag of getStack(project); track tag) {
            <span class="tag-pill">{{ tag }}</span>
          }
        </div>
        <h3 class="project-heading">{{ project.title }}</h3>
        <p class="project-desc">{{ project.description }}</p>
        <a [href]="project.liveUrl || project.githubUrl" class="project-action" target="_blank">
          View Details
          <div class="arrow-container">
            <span class="material-symbols-outlined icon">north_east</span>
          </div>
        </a>
      </div>
    </div>
  }
}
</div>
```

---

### STEP 10 — Wire Contact Form
**File:** `frontend/src/app/public/pages/contact/contact.component.ts`

```typescript
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';

@Component({
  selector: 'app-contact', 
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
  templateUrl: './contact.component.html', 
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent {
  form = { name: '', email: '', subject: '', content: '' };
  loading = false;
  success = false;
  error = '';

  constructor(private api: ApiService) {}

  onSubmit(event: Event): void {
    event.preventDefault();
    if (!this.form.name || !this.form.email || !this.form.content) {
      this.error = 'Please fill in all required fields.';
      return;
    }
    this.loading = true;
    this.error = '';
    this.api.sendMessage(this.form).subscribe({
      next: () => { 
        this.success = true; 
        this.loading = false; 
        this.form = { name: '', email: '', subject: '', content: '' }; 
      },
      error: () => { 
        this.error = 'Failed to send. Please try again.'; 
        this.loading = false; 
      }
    });
  }
}
```

In `contact.component.html`, bind form fields:
```html
<!-- Add to each input -->
[(ngModel)]="form.name"
[(ngModel)]="form.email"
[(ngModel)]="form.subject"
[(ngModel)]="form.content"

<!-- Success/error messages -->
@if (success) { <div class="success-msg">Message sent! I'll get back to you soon.</div> }
@if (error) { <div class="error-msg">{{ error }}</div> }

<!-- Submit button -->
<button type="submit" [disabled]="loading">{{ loading ? 'Sending...' : 'Send Message' }}</button>
```

---

### STEP 11 — Wire Admin Messages
**File:** `frontend/src/app/admin/pages/messages/messages.component.ts`

Replace entire component with:
```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { Message, PagedResult } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-admin-messages', 
  standalone: true, 
  imports: [CommonModule],
  templateUrl: './messages.component.html', 
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  selectedMessage: Message | null = null;
  loading = true;
  totalCount = 0;
  page = 1;
  pageSize = 20;

  constructor(private api: ApiService) {}

  ngOnInit(): void { this.loadMessages(); }

  loadMessages(): void {
    this.loading = true;
    this.api.getMessages(this.page, this.pageSize).subscribe({
      next: (res: PagedResult<Message>) => { 
        this.messages = res.items; 
        this.totalCount = res.totalCount; 
        this.loading = false; 
      },
      error: () => { this.loading = false; }
    });
  }

  selectMessage(msg: Message): void {
    this.selectedMessage = msg;
    if (!msg.isRead) {
      this.api.markMessageAsRead(msg.id).subscribe();
      msg.isRead = true;
    }
  }

  deleteMessage(id: number): void {
    if (!confirm('Delete this message?')) return;
    this.api.deleteMessage(id).subscribe(() => {
      this.messages = this.messages.filter(m => m.id !== id);
      if (this.selectedMessage?.id === id) this.selectedMessage = null;
    });
  }

  onPageChange(newPage: number): void {
    this.page = newPage;
    this.loadMessages();
  }
}
```

Fix template field names:
- `msg.name` instead of `msg.senderName`
- `msg.isRead` instead of `!msg.isUnread`
- `msg.createdAt` instead of `msg.time`
- `msg.content` for the message body

---

### STEP 12 — Wire Admin Projects
**File:** `frontend/src/app/admin/pages/projects/projects.component.ts`

```typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { Project } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-admin-projects', 
  standalone: true, 
  imports: [CommonModule],
  templateUrl: './projects.component.html', 
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  projects: Project[] = [];
  loading = true;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.api.getAllProjectsAdmin().subscribe({
      next: (data) => { this.projects = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  getStack(p: Project): string[] {
    try { return JSON.parse(p.stack); } catch { return []; }
  }

  deleteProject(id: number): void {
    if (!confirm('Delete this project?')) return;
    this.api.deleteProject(id).subscribe(() => {
      this.projects = this.projects.filter(p => p.id !== id);
    });
  }

  togglePublish(p: Project): void {
    const action = p.status === 'Production'
      ? this.api.unpublishProject(p.id)
      : this.api.publishProject(p.id);
    action.subscribe(() => {
      p.status = p.status === 'Production' ? 'Draft' : 'Production';
    });
  }
}
```

Fix template: use `project.title`, `project.year`, `project.status`, `getStack(project)` for tags.

---

### STEP 13 — Update ApiService
**File:** `frontend/src/app/core/services/api.service.ts`

Changes needed:
```typescript
import { PagedResult, PortfolioConfig, Project, Message } from '../models/portfolio.models';

// 1. Fix return type of getPortfolioConfig
getPortfolioConfig(): Observable<PortfolioConfig> {   // was Observable<any>
  return this.http.get<PortfolioConfig>(`${this.API_URL}/portfolio/config`);
}

// 2. Add admin all-projects method
getAllProjectsAdmin(): Observable<Project[]> {
  return this.http.get<Project[]>(`${this.API_URL}/portfolio/admin/projects`);
}

// 3. Fix updateProject — use {id} in URL
updateProject(project: Project): Observable<Project> {
  return this.http.put<Project>(`${this.API_URL}/portfolio/projects/${project.id}`, project);
}

// 4. Fix getMessages to return PagedResult
getMessages(page: number = 1, pageSize: number = 20): Observable<PagedResult<Message>> {
  return this.http.get<PagedResult<Message>>(
    `${this.API_URL}/messages?page=${page}&pageSize=${pageSize}`
  );
}

// 5. Add auth token getter (for interceptor)
getToken(): string | null {
  return localStorage.getItem('token');
}
```

---

### ✅ STEP 14 — Backend: Verify Endpoints (Already Implemented)

#### 14a. ✅ `PUT /api/auth/password`
**Status:** ALREADY IMPLEMENTED  
**File:** `webapi/Portfolio.Api/Controllers/AuthController.cs`

The password change endpoint already exists with proper implementation using MediatR.

#### 14b. ✅ `GET/PUT /api/settings`
**Status:** ALREADY IMPLEMENTED  
**File:** `webapi/Portfolio.Api/Controllers/SettingsController.cs`

Full implementation exists with MediatR handlers for Get and Update operations.

#### 14c. ✅ Admin all-projects
**Status:** IMPLEMENTED  
**File:** `webapi/Portfolio.Api/Controllers/PortfolioController.cs`

```csharp
[HttpGet("admin/projects")]
[Authorize]
public async Task<IActionResult> GetAllProjectsAdmin()
{
    var projects = await _context.Projects
        .OrderByDescending(p => p.IsFeatured)
        .ThenBy(p => p.DisplayOrder)
        .ToListAsync();
    return Ok(projects);
}
```

#### 14d. ✅ Fix `dashboard-stats` — add `totalMessages`
**Status:** IMPLEMENTED  
**File:** `PortfolioController.cs`

`totalMessages` has been added to the dashboard stats response.

---

### STEP 15 — Add Serilog Logging to Backend ⭐ NEW

#### 15a. Install Serilog Packages
**File:** `webapi/Portfolio.Api/Portfolio.Api.csproj`

Add these package references:
```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
```

#### 15b. Configure Serilog in Program.cs
**File:** `webapi/Portfolio.Api/Program.cs`

Add at the top of the file:
```csharp
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
    
    // ... rest of existing code ...
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
```

#### 15c. Add Request Logging
Add after `var app = builder.Build();`:
```csharp
// Add request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});
```

#### 15d. Add .gitignore for logs
Add to `webapi/.gitignore`:
```
logs/
*.log
```

---

## Checklist

| # | Task | File(s) | Status |
|---|---|---|---|
| 1 | Fix portfolio.models.ts | `core/models/portfolio.models.ts` | ⬜ PENDING |
| 2 | Create auth.guard.ts | `core/guards/auth.guard.ts` | ⬜ PENDING |
| 3 | Create HTTP Interceptor ⭐ | `core/interceptors/auth.interceptor.ts` | ⬜ PENDING |
| 4 | Register interceptor in app.config.ts ⭐ | `app.config.ts` | ⬜ PENDING |
| 5 | Protect admin routes | `admin/admin.routes.ts` | ⬜ PENDING |
| 6 | Wire HomeComponent | `public/pages/home/home.component.ts` | ⬜ PENDING |
| 7 | Wire Hero component | `home/components/hero/hero.component.ts+html` | ⬜ PENDING |
| 8 | Wire About component | `home/components/about/about.component.ts+html` | ⬜ PENDING |
| 9 | Wire Skills component | `home/components/skills/skills.component.ts+html` | ⬜ PENDING |
| 10 | Wire Projects component | `home/components/projects/projects.component.ts+html` | ⬜ PENDING |
| 11 | Wire Contact form | `public/pages/contact/contact.component.ts+html` | ⬜ PENDING |
| 12 | Wire Admin Messages | `admin/pages/messages/messages.component.ts+html` | ⬜ PENDING |
| 13 | Wire Admin Projects | `admin/pages/projects/projects.component.ts+html` | ⬜ PENDING |
| 14 | Fix ApiService types | `core/services/api.service.ts` | ⬜ PENDING |
| 15a | Backend: PUT /auth/password | `AuthController.cs` | ✅ DONE |
| 15b | Backend: GET/PUT /settings | `SettingsController.cs` | ✅ DONE |
| 15c | Backend: GET /portfolio/admin/projects | `PortfolioController.cs` | ✅ DONE |
| 15d | Backend: fix dashboard-stats totalMessages | `PortfolioController.cs` | ✅ DONE |
| 16a | Backend: Add Serilog packages ⭐ | `Portfolio.Api.csproj` | ⬜ PENDING |
| 16b | Backend: Configure Serilog ⭐ | `Program.cs` | ⬜ PENDING |
| 16c | Backend: Add request logging ⭐ | `Program.cs` | ⬜ PENDING |
| 16d | Backend: Ignore logs in git ⭐ | `.gitignore` | ⬜ PENDING |

---

## Smoke Tests (after integration)

```bash
# 1. Backend serves with logging
# Check console output for Serilog logs
dotnet run --project webapi/Portfolio.Api

# 2. Backend returns config
curl http://localhost:5000/api/portfolio/config

# 3. Login works
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"Menomo","password":"Menomo@123"}'

# 4. Unauthenticated access blocked
curl http://localhost:5000/api/messages   # Should return 401

# 5. Authenticated access works (replace <token>)
curl http://localhost:5000/api/messages \
  -H "Authorization: Bearer <token>"
```

**Browser checks:**
1. `http://localhost:4200` — Home page renders real API data (no hardcoded text)
2. `http://localhost:4200/admin` — Redirects to `/login` (guard works)
3. Login → directed to `/admin/dashboard` with real stats
4. Submit contact form → message appears in `/admin/messages`
5. `/admin/messages` loads real messages from database
6. `/admin/projects` loads real projects from database
7. Check browser DevTools Network tab — all API calls succeed (200)
8. Check backend console/logs — requests are logged with timing

---

## Troubleshooting

### Issue: 401 Unauthorized on API calls
**Cause:** JWT token not being sent or expired  
**Fix:** Verify `authInterceptorFn` is registered in `app.config.ts` and `AuthService.getToken()` returns the stored token.

### Issue: CORS errors in browser
**Cause:** Frontend origin not allowed  
**Fix:** Verify backend `Program.cs` has `http://localhost:4200` in CORS policy.

### Issue: Images not loading
**Cause:** Image URL path incorrect  
**Fix:** Verify `getImageUrl()` methods prepend `http://localhost:5000/` correctly.

### Issue: Skills/projects not displaying
**Cause:** Field name mismatch  
**Fix:** Check browser console for errors, verify field names match the mismatch table above.

### Issue: No Serilog output
**Cause:** Serilog not configured correctly  
**Fix:** Verify `builder.Host.UseSerilog()` is called and `logs/` directory is writable.

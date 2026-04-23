import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ProjectConfig } from '../../../../../core/models/portfolio.models';
import { ApiService } from '../../../../../core/services/api.service';
import { environment } from '../../../../../../environments/environment';

interface Project {
  id: number;
  name: string;
  category: string;
  status: 'live' | 'wip';
  year: string;
  description: string;
  stack: string[];
  githubUrl: string;
  linkedinUrl?: string;
  liveUrl?:string;
  featured?: boolean;
  gradient: string;
  icon?: string;
  image?:string;
}

@Component({
  selector: 'app-home-projects',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  @Input() featuredProjects: ProjectConfig[] = [];
  @Input() moreProjects: ProjectConfig[] = [];

  projects: Project[] = [
    {
      id: 1,
      name: 'MenoPro — Gym Management',
      category: 'Web Application',
      status: 'live',
      year: '2025',
      description: 'Premium gym platform with member & trainer portals, workout plans, diet tracking, and Chart.js analytics. Includes Glassmorphism UI, Stripe payments, and Gemini AI integration.',
      stack: ['ASP.NET Core MVC', 'SQL Server', 'Entity Framework', 'Stripe', 'Gemini AI'],
      githubUrl: 'https://github.com/abdallah670/GymMVC',
      linkedinUrl: 'https://www.linkedin.com/posts/abdullah-mohammed-334475294_aspnetcore-csharp-webdevelopment-activity-7424228685093994496-wSwl',
      featured: true,
      gradient: 'linear-gradient(90deg,#4ade80,#22d3ee,transparent)',
      icon: '🏋'
    },
    {
      id: 2,
      name: 'Labor Marketplace System',
      category: 'Full-Stack Platform',
      status: 'live',
      year: '2026',
      description: 'Platform connecting workers with job posters. Multi-role auth, real-time chat via SignalR, Stripe Connect payments, Hangfire background jobs, and geographic search with spatial SQL queries.',
      stack: ['ASP.NET Core MVC', '.NET 9', 'SignalR', 'Stripe Connect', 'Hangfire'],
      githubUrl: 'https://github.com/abdallah670/LaborMVC',
      linkedinUrl: 'https://www.linkedin.com/posts/abdullah-mohammed-334475294_dotnet-architecture-systemdesign-activity-7444313186763358208-gBDD',
      featured: true,
      gradient: 'linear-gradient(90deg,#3b82f6,#818cf8,transparent)',
      icon: '🔗'
    },
    {
      id: 3,
      name: 'Outfit Planner',
      category: 'Web Application',
      status: 'wip',
      year: '2026',
      description: 'Intelligent wardrobe management generating outfit suggestions based on real-time weather, occasion, and personal style. Built with Clean Architecture and CQRS pattern.',
      stack: ['ASP.NET Core 9', 'Angular 17+', 'NgRx', 'Clean Architecture', 'CQRS'],
      githubUrl: 'https://github.com/abdallah670/Outfit-Planner',
      gradient: 'linear-gradient(90deg,#f472b6,#a78bfa,transparent)',
      icon: '👗'
    }
  ];

  private isBrowser: boolean;

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private api: ApiService
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      this.initScrollReveal();
    }
  }

  private initScrollReveal(): void {
    if ('IntersectionObserver' in window) {
      const revealObs = new IntersectionObserver((entries) => {
        entries.forEach(e => { 
          if (e.isIntersecting) { 
            e.target.classList.add('visible'); 
            revealObs.unobserve(e.target); 
          } 
        });
      }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });
      
      setTimeout(() => {
        document.querySelectorAll('#projects .reveal').forEach(el => revealObs.observe(el));
      }, 100);
    }
  }

  get allProjects(): Project[] {
    if (this.featuredProjects?.length || this.moreProjects?.length) {
      return [
        ...this.featuredProjects.map(p => this.mapProjectConfig(p, true)),
        ...this.moreProjects.map(p => this.mapProjectConfig(p, false))
      ];
    }
    return this.projects;
  }

  private mapProjectConfig(p: ProjectConfig, isFeatured: boolean): Project {
    let stack: string[] = [];
    try {
      stack = JSON.parse(p.stack);
    } catch {
      stack = [];
    }

    return {
      id: p.id,
      name: p.title,
      category: p.category || 'Web Application',
      status: p.status === 'Production' ? 'live' : 'wip',
      year: p.year || '2025',
      description: p.description || '',
      stack: stack,
      githubUrl: p.githubUrl || '#',
      linkedinUrl: p.linkedinUrl,
      liveUrl :p.LiveUrl,
      featured: isFeatured || p.isFeatured,
      gradient: this.getGradient(isFeatured),
      icon:this.getIcon(p.title),
      image:p.image?this.getImageUrl(p.image):this.getIcon(p.title)
    };
  }

  private getGradient(isFeatured: boolean): string {
    return isFeatured 
      ? 'linear-gradient(90deg,#4ade80,#22d3ee,transparent)'
      : 'linear-gradient(90deg,#3b82f6,#818cf8,transparent)';
  }

  private getIcon(title: string): string {
    
    const titleLower = title.toLowerCase();
    if (titleLower.includes('gym') || titleLower.includes('fitness')) return '🏋';
    if (titleLower.includes('labor') || titleLower.includes('job')) return '🔗';
    if (titleLower.includes('outfit') || titleLower.includes('wardrobe')) return '👗';
    return '💻';
  }

  getStack(project: ProjectConfig): string[] {
    try { return JSON.parse(project.stack); }
    catch { return []; }
  }

  getImageUrl(path: string | null | undefined): string {
    if (!path) return '';
    if (path.startsWith('http')) return path;
    return `${environment.baseUrl}/${path}`;
  }

  onProjectLinkClick(projectId: number, event: Event): void {
    this.api.incrementProjectViews(projectId).subscribe({
      next: () => console.log(`View tracked for project ${projectId}`),
      error: (err) => console.error('Failed to track view:', err)
    });
  }
}

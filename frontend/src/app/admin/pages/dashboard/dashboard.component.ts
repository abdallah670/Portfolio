import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { DashboardStats, Project, SkillCategory, Message } from '../../../core/models/portfolio.models';
import Chart from 'chart.js/auto';
import { Router } from '@angular/router';

interface RecentProject {
  id: number;
  title: string;
  description: string;
  stack: string;
  status: string;
  image: string;
  year: string;
  category: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('skillsChart') skillsChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('messagesChart') messagesChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('viewsChart') viewsChartRef!: ElementRef<HTMLCanvasElement>;

  stats: DashboardStats | null = null;
  recentProjects: RecentProject[] = [];
  recentMessages: Message[] = [];
  skillCategories: SkillCategory[] = [];

  skillsChart!: Chart;
  messagesChart!: Chart;
  viewsChart!: Chart;

  constructor(
    public apiService: ApiService,
    public router: Router,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDashboardStats();
    this.loadRecentProjects();
    this.loadRecentMessages();
    this.loadSkillCategories();
   
  }

  ngAfterViewInit(): void {
    // Charts created after data loads
  }

  
  
  loadDashboardStats(): void {
    this.apiService.getDashboardStats().subscribe({
      next: (stats) => {
        this.stats = stats;
        // Recent projects from stats (already limited to 5)
        this.recentProjects = stats.recentProjects;
        // Create monthly charts after data loads
        this.cd.detectChanges();
        setTimeout(() => {
          this.createMessagesChart();
          this.createViewsChart();
        }, 0);
      },
      error: (err) => {
        console.error('Failed to load dashboard stats:', err);
      }
    });
  }

  loadRecentProjects(): void {
    this.apiService.getAllProjectsAdmin().subscribe({
      next: (projects) => {
        // recentProjects already set from dashboard stats, but this is backup/refresh
        if (!this.recentProjects.length && projects.items.length) {
          this.recentProjects = projects.items.slice(0, 5);
        }
      },
      error: () => {}
    });
  }

  loadRecentMessages(): void {
    // Get first page with small pageSize to fetch latest messages
    this.apiService.getMessages(1, 5).subscribe({
      next: (res) => {
        this.recentMessages = res.items;
      },
      error: () => {}
    });
  }

  loadSkillCategories(): void {
    this.apiService.getSkills().subscribe({
      next: (categories) => {
        this.skillCategories = categories;
        // Force change detection to ensure canvas is rendered
        this.cd.detectChanges();
        // Defer chart creation to next tick to ensure DOM is ready
        setTimeout(() => this.createSkillsChart(), 0);
      },
      error: () => {}
    });
  }

  private createSkillsChart(): void {
    if (!this.skillsChartRef || !this.skillCategories.length) return;
    const ctx = this.skillsChartRef.nativeElement.getContext('2d');
    if (!ctx) return;

    // Prepare data: avg skill level per category
    const labels = this.skillCategories.map(c => c.title);
    const data = this.skillCategories.map(c => {
      if (!c.skills || !c.skills.length) return 0;
      const sum = c.skills.reduce((acc, s) => acc + (s.level || 0), 0);
      return Math.round(sum / c.skills.length);
    });

    // Generate colors from category color property
    const backgroundColors = this.skillCategories.map(c => {
      const hex = c.color || '#9ba8ff';
      return hex + 'CC'; // Add alpha
    });

    this.skillsChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Avg Skill Level %',
          data,
          backgroundColor: backgroundColors,
          borderRadius: 6
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'y',
        plugins: { legend: { display: false } },
        scales: {
          x: {
            max: 100,
            grid: { color: 'rgba(128, 128, 128, 0.1)' },
            ticks: { font: { size: 10 } }
          },
          y: {
            grid: { display: false },
            ticks: { font: { size: 11 } }
          }
        }
      }
    });
  }


  private createMessagesChart(): void {
    if (!this.messagesChartRef || !this.stats?.messagesByMonth?.length) return;
    const ctx = this.messagesChartRef.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = this.stats.messagesByMonth.map(m => {
      const [year, month] = m.month.split('-');
      const date = new Date(parseInt(year), parseInt(month) - 1);
      return date.toLocaleDateString('en-US', { month: 'short', year: '2-digit' });
    });
    const data = this.stats.messagesByMonth.map(m => m.count);

    this.messagesChart = new Chart(ctx, {
      type: 'line',
      data: {
        labels,
        datasets: [{
          label: 'Messages',
          data,
          borderColor: '#10b981',
          backgroundColor: 'rgba(16, 185, 129, 0.1)',
          fill: true,
          tension: 0.4
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
          y: {
            beginAtZero: true,
            ticks: { stepSize: 1 },
            grid: { color: 'rgba(128, 128, 128, 0.1)' }
          },
          x: {
            grid: { display: false }
          }
        }
      }
    });
  }

  
  

  getImageUrl(path: string): string {
    if (!path) return '';
    if (path.startsWith('http')) return path;
    return `http://localhost:5000/${path}`;
  }

  viewProject(projectId: number): void {
    this.router.navigate(['/admin/projects'], { queryParams: { view: projectId } });
  }

  viewMessage(messageId: number): void {
    this.router.navigate(['/admin/messages'], { queryParams: { message: messageId } });
  }


  private createViewsChart(): void {
    if (!this.viewsChartRef || !this.stats?.viewsByMonth?.length) return;
    const ctx = this.viewsChartRef.nativeElement.getContext('2d');
    if (!ctx) return;

    const labels = this.stats.viewsByMonth.map(p => p.name);
    const data = this.stats.viewsByMonth.map(p => p.views);

    this.viewsChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Views',
          data,
          backgroundColor: '#f59e0b',
          borderRadius: 6
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        indexAxis: 'y',
        plugins: { legend: { display: false } },
        scales: {
          x: {
            beginAtZero: true,
            grid: { color: 'rgba(128, 128, 128, 0.1)' }
          },
          y: {
            grid: { display: false },
            ticks: { font: { size: 11 } }
          }
        }
      }
    });
  }

  ngOnDestroy(): void {
    this.skillsChart?.destroy();
    this.messagesChart?.destroy();
    this.viewsChart?.destroy();
  }
}

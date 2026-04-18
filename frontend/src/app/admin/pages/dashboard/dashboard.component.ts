import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { DashboardStats, Project, SkillCategory } from '../../../core/models/portfolio.models';
import Chart from 'chart.js/auto';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('skillsChart') skillsChartRef!: ElementRef<HTMLCanvasElement>;

  stats: DashboardStats | null = null;
  recentProjects: Project[] = [];
  skillCategories: SkillCategory[] = [];
  currentTime = '';

  skillsChart!: Chart;

  constructor(
    public apiService: ApiService,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboardStats();
    this.loadRecentProjects();
    this.loadSkillCategories();
    this.updateTime();
    setInterval(() => this.updateTime(), 1000);
  }

  ngAfterViewInit(): void {
    // Charts created after data loads
  }

  
  
  loadDashboardStats(): void {
    this.apiService.getDashboardStats().subscribe({
      next: (stats) => {
        this.stats = stats;
      },
      error: (err) => {
        console.error('Failed to load dashboard stats:', err);
      }
    });
  }

  loadRecentProjects(): void {
    this.apiService.getAllProjectsAdmin().subscribe({
      next: (projects) => {
        this.recentProjects = projects.slice(0, 5);
      },
      error: () => {}
    });
  }

  loadSkillCategories(): void {
    this.apiService.getSkills().subscribe({
      next: (categories) => {
        this.skillCategories = categories;
        this.createSkillsChart();
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

  updateTime(): void {
    const now = new Date();
    this.currentTime = now.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: true
    }) + ' PST';
  }

  formatNumber(num: number): string {
    if (num >= 1000) {
      return (num / 1000).toFixed(1) + 'k';
    }
    return num.toString();
  }

  ngOnDestroy(): void {
    this.skillsChart?.destroy();
  }
}

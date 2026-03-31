import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { DashboardStats, Project, Message } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard">
      <!-- Page Header -->
      <section class="page-header">
        <div>
          <span class="section-label">Operational Hub</span>
          <h1>Systems <span class="highlight">Overview</span></h1>
        </div>
        <div class="sync-time">
          <p class="label">Last Synced</p>
          <p class="time">{{ currentTime }}</p>
        </div>
      </section>

      <!-- Metrics Grid -->
      <section class="metrics-grid">
        <!-- Metric 1 -->
        <div class="metric-card">
          <div class="metric-content">
            <p class="metric-label">Page Views</p>
            <h3 class="metric-value">{{ formatNumber(stats?.profileViews || 0) }}</h3>
            <div class="metric-trend up">
              <span class="material-symbols-outlined">trending_up</span>
              <span>+12.4%</span>
            </div>
          </div>
          <div class="sparkline">
            <svg viewBox="0 0 100 30">
              <path d="M0 25 Q 10 20, 20 22 T 40 15 T 60 18 T 80 10 T 100 5" 
                    fill="none" stroke="#00e3fd" stroke-width="2"/>
            </svg>
          </div>
        </div>

        <!-- Metric 2 -->
        <div class="metric-card">
          <div class="metric-content">
            <p class="metric-label">Project Count</p>
            <h3 class="metric-value">{{ stats?.totalProjects || 0 }}</h3>
            <div class="metric-trend neutral">
              <span class="material-symbols-outlined">rocket_launch</span>
              <span>{{ stats?.draftProjects || 0 }} Drafts</span>
            </div>
          </div>
          <div class="sparkline">
            <svg viewBox="0 0 100 30">
              <path d="M0 10 Q 25 15, 50 12 T 100 20" 
                    fill="none" stroke="#9ba8ff" stroke-width="2"/>
            </svg>
          </div>
        </div>

        <!-- Metric 3 -->
        <div class="metric-card">
          <div class="metric-content">
            <p class="metric-label">Total Messages</p>
            <h3 class="metric-value">{{ stats?.totalMessages || 0 }}</h3>
            <div class="metric-trend attention">
              <span class="material-symbols-outlined">mail</span>
              <span>{{ stats?.unreadMessages || 0 }} Unread</span>
            </div>
          </div>
          <div class="sparkline">
            <svg viewBox="0 0 100 30">
              <path d="M0 20 L 20 15 L 40 18 L 60 10 L 80 15 L 100 8" 
                    fill="none" stroke="#ff6e84" stroke-width="2"/>
            </svg>
          </div>
        </div>

        <!-- Metric 4 -->
        <div class="metric-card">
          <div class="metric-content">
            <p class="metric-label">Tech Categories</p>
            <h3 class="metric-value">{{ stats?.skillCategories || 0 }}</h3>
            <div class="metric-trend up">
              <span class="material-symbols-outlined">group</span>
              <span>Core Stack</span>
            </div>
          </div>
          <div class="sparkline">
            <svg viewBox="0 0 100 30">
              <path d="M0 25 C 20 25, 40 5, 100 5" 
                    fill="none" stroke="#00e3fd" stroke-width="2"/>
            </svg>
          </div>
        </div>
      </section>

      <!-- Main Content Grid -->
      <section class="content-grid">
        <!-- Recent Activity -->
        <div class="panel activity-panel">
          <div class="panel-header">
            <h3>Recent Activity</h3>
            <button class="btn-link">View All Archive</button>
          </div>
          <div class="activity-list">
            @for (activity of activities; track activity.id) {
              <div class="activity-item">
                <div class="activity-icon" [class]="activity.type">
                  <span class="material-symbols-outlined">{{ activity.icon }}</span>
                </div>
                <div class="activity-content">
                  <p class="activity-title" [innerHTML]="activity.title"></p>
                  <p class="activity-desc">{{ activity.description }}</p>
                </div>
                <div class="activity-time">{{ activity.time }}</div>
              </div>
            }
          </div>
        </div>

        <!-- Traffic Sources -->
        <div class="panel traffic-panel">
          <div class="panel-header">
            <h3>Traffic Sources</h3>
          </div>
          <div class="traffic-list">
            <div class="traffic-item">
              <div class="traffic-label">
                <span>Organic Search</span>
                <span class="value">48%</span>
              </div>
              <div class="progress-bar">
                <div class="progress" style="width: 48%"></div>
              </div>
            </div>
            <div class="traffic-item">
              <div class="traffic-label">
                <span>Social Media</span>
                <span class="value">24%</span>
              </div>
              <div class="progress-bar">
                <div class="progress primary" style="width: 24%"></div>
              </div>
            </div>
            <div class="traffic-item">
              <div class="traffic-label">
                <span>Referral</span>
                <span class="value">18%</span>
              </div>
              <div class="progress-bar">
                <div class="progress tertiary" style="width: 18%"></div>
              </div>
            </div>
            <div class="traffic-item">
              <div class="traffic-label">
                <span>Direct</span>
                <span class="value">10%</span>
              </div>
              <div class="progress-bar">
                <div class="progress muted" style="width: 10%"></div>
              </div>
            </div>
          </div>
          
          <!-- AI Prediction Card -->
          <div class="ai-card">
            <div class="ai-header">
              <span class="material-symbols-outlined">auto_awesome</span>
              <span>AI Prediction</span>
            </div>
            <p>Based on historical patterns, your organic traffic is expected to grow by <span class="highlight">8.2%</span> next week due to recent SEO optimization.</p>
          </div>
        </div>
      </section>

      <!-- Featured Project Card -->
      <section class="featured-section">
        <div class="featured-card">
          <div class="featured-content">
            <span class="featured-label">Project Highlight</span>
            <h2>Neural Interface<br/>Explorer v4</h2>
            <p>The latest evolution of your portfolio management system. Currently trending with active development and modern architectural patterns.</p>
            <div class="featured-actions">
              <button class="btn-primary" routerLink="/admin/analytics">Analytics</button>
              <button class="btn-secondary" routerLink="/admin/projects">View Projects</button>
            </div>
          </div>
          <div class="featured-visual">
            <div class="code-preview">
              <pre><code [innerHTML]="codePreview"></code></pre>
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .dashboard {
      display: flex;
      flex-direction: column;
      gap: 32px;
      animation: fadeIn 0.5s ease;
    }

    // Page Header
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-end;
      margin-bottom: 16px;
    }

    .section-label {
      font-family: var(--font-headline);
      font-size: 14px;
      font-weight: 600;
      color: var(--secondary);
      text-transform: uppercase;
      letter-spacing: 0.2em;
      display: block;
      margin-bottom: 8px;
    }

    .page-header h1 {
      font-family: var(--font-headline);
      font-size: 48px;
      font-weight: 800;
      color: var(--on-background);
      letter-spacing: -0.02em;
    }

    .page-header .highlight {
      color: var(--primary-dim);
    }

    .sync-time {
      text-align: right;
    }

    .sync-time .label {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      color: var(--on-surface-variant);
      margin-bottom: 4px;
    }

    .sync-time .time {
      font-family: var(--font-headline);
      font-size: 16px;
      font-weight: 700;
      color: var(--on-surface);
    }

    // Metrics Grid
    .metrics-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: 24px;
    }

    .metric-card {
      background: var(--surface-container-high);
      border-radius: var(--radius-lg);
      padding: 24px;
      position: relative;
      overflow: hidden;
      transition: transform 0.2s ease;
    }

    .metric-card:hover {
      transform: translateY(-2px);
    }

    .metric-content {
      position: relative;
      z-index: 1;
    }

    .metric-label {
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      color: var(--on-surface-variant);
      margin-bottom: 16px;
    }

    .metric-value {
      font-family: var(--font-headline);
      font-size: 32px;
      font-weight: 800;
      color: var(--on-surface);
      margin-bottom: 8px;
    }

    .metric-trend {
      display: flex;
      align-items: center;
      gap: 6px;
      font-size: 12px;
      font-weight: 600;
    }

    .metric-trend.up {
      color: var(--secondary);
    }

    .metric-trend.neutral {
      color: var(--primary);
    }

    .metric-trend.attention {
      color: var(--error);
    }

    .metric-trend .material-symbols-outlined {
      font-size: 16px;
    }

    .sparkline {
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      height: 60px;
      opacity: 0.3;
      transition: opacity 0.2s ease;
    }

    .metric-card:hover .sparkline {
      opacity: 0.6;
    }

    .sparkline svg {
      width: 100%;
      height: 100%;
    }

    // Content Grid
    .content-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 32px;
    }

    // Panel Styles
    .panel {
      background: var(--surface-container-low);
      border-radius: var(--radius-lg);
      overflow: hidden;
    }

    .panel-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px;
      border-bottom: 1px solid var(--outline-variant);
    }

    .panel-header h3 {
      font-family: var(--font-headline);
      font-size: 20px;
      font-weight: 700;
      color: var(--on-surface);
    }

    .btn-link {
      font-size: 12px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      color: var(--on-surface-variant);
      background: none;
      border: none;
      cursor: pointer;
      transition: color 0.2s ease;
    }

    .btn-link:hover {
      color: var(--secondary);
    }

    // Activity List
    .activity-list {
      padding: 8px;
    }

    .activity-item {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 16px;
      border-radius: var(--radius-md);
      transition: background 0.2s ease;
      cursor: pointer;
    }

    .activity-item:hover {
      background: var(--surface-container-high);
    }

    .activity-icon {
      width: 48px;
      height: 48px;
      border-radius: var(--radius-lg);
      background: var(--surface-container-highest);
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      transition: transform 0.2s ease;
    }

    .activity-item:hover .activity-icon {
      transform: scale(1.1);
    }

    .activity-icon.commit {
      color: var(--secondary);
    }

    .activity-icon.inquiry {
      color: var(--primary);
    }

    .activity-icon.traffic {
      color: var(--tertiary-fixed-dim);
    }

    .activity-icon.warning {
      color: var(--error);
    }

    .activity-icon .material-symbols-outlined {
      font-size: 24px;
    }

    .activity-content {
      flex: 1;
      min-width: 0;
    }

    .activity-title {
      font-size: 14px;
      font-weight: 500;
      color: var(--on-surface);
      margin-bottom: 4px;
    }

    .activity-title :deep(strong) {
      color: var(--primary);
    }

    .activity-desc {
      font-size: 11px;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--on-surface-variant);
    }

    .activity-time {
      font-size: 12px;
      font-weight: 700;
      color: var(--on-surface);
      white-space: nowrap;
    }

    // Traffic Panel
    .traffic-panel {
      padding: 24px;
    }

    .traffic-list {
      display: flex;
      flex-direction: column;
      gap: 24px;
      margin-bottom: 32px;
    }

    .traffic-item {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .traffic-label {
      display: flex;
      justify-content: space-between;
      font-size: 12px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      color: var(--on-surface);
    }

    .traffic-label .value {
      font-family: var(--font-headline);
      color: var(--secondary);
    }

    .progress-bar {
      height: 8px;
      background: var(--surface-container-high);
      border-radius: var(--radius-full);
      overflow: hidden;
    }

    .progress {
      height: 100%;
      background: var(--secondary);
      border-radius: var(--radius-full);
      box-shadow: 0 0 12px rgba(0, 227, 253, 0.4);
    }

    .progress.primary {
      background: var(--primary);
      box-shadow: 0 0 12px rgba(155, 168, 255, 0.4);
    }

    .progress.tertiary {
      background: var(--tertiary-fixed-dim);
      box-shadow: 0 0 12px rgba(203, 208, 239, 0.4);
    }

    .progress.muted {
      background: var(--on-surface-variant);
      opacity: 0.5;
      box-shadow: none;
    }

    // AI Card
    .ai-card {
      background: var(--surface-bright);
      border: 1px solid var(--outline-variant);
      border-radius: var(--radius-lg);
      padding: 24px;
    }

    .ai-header {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 16px;
    }

    .ai-header .material-symbols-outlined {
      color: var(--secondary);
      font-size: 20px;
    }

    .ai-header span:last-child {
      font-size: 12px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      color: var(--on-surface);
    }

    .ai-card p {
      font-size: 13px;
      line-height: 1.6;
      color: var(--on-surface-variant);
    }

    .ai-card .highlight {
      color: var(--secondary);
      font-weight: 600;
    }

    // Featured Section
    .featured-section {
      margin-top: 16px;
    }

    .featured-card {
      background: var(--surface-container-low);
      border-radius: var(--radius-lg);
      overflow: hidden;
      display: flex;
      border: 1px solid var(--outline-variant);
    }

    .featured-content {
      flex: 1;
      padding: 48px;
      position: relative;
      z-index: 1;
    }

    .featured-label {
      font-family: var(--font-headline);
      font-size: 14px;
      font-weight: 600;
      color: var(--secondary);
      text-transform: uppercase;
      letter-spacing: 0.1em;
      display: block;
      margin-bottom: 16px;
    }

    .featured-content h2 {
      font-family: var(--font-headline);
      font-size: 40px;
      font-weight: 800;
      color: var(--on-surface);
      line-height: 1.1;
      margin-bottom: 24px;
      letter-spacing: -0.02em;
    }

    .featured-content p {
      font-size: 16px;
      line-height: 1.6;
      color: var(--on-surface-variant);
      max-width: 500px;
      margin-bottom: 32px;
    }

    .featured-actions {
      display: flex;
      gap: 16px;
    }

    .btn-primary {
      padding: 14px 32px;
      background: var(--primary);
      color: var(--on-primary-fixed);
      border-radius: var(--radius-lg);
      font-family: var(--font-headline);
      font-size: 12px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      border: none;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .btn-primary:hover {
      transform: scale(1.05);
    }

    .btn-secondary {
      padding: 14px 32px;
      background: transparent;
      border: 1px solid var(--outline-variant);
      color: var(--on-surface);
      border-radius: var(--radius-lg);
      font-family: var(--font-headline);
      font-size: 12px;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .btn-secondary:hover {
      background: var(--surface-container-high);
    }

    .featured-visual {
      flex: 1;
      position: relative;
      overflow: hidden;
    }

    .code-preview {
      position: absolute;
      inset: 0;
      background: var(--surface-container-highest);
      padding: 32px;
      overflow: hidden;
    }

    .code-preview::before {
      content: '';
      position: absolute;
      left: 0;
      top: 0;
      bottom: 0;
      width: 200px;
      background: linear-gradient(to right, var(--surface-container-low), transparent);
      z-index: 1;
    }

    .code-preview pre {
      margin: 0;
      font-family: 'JetBrains Mono', 'Fira Code', monospace;
      font-size: 13px;
      line-height: 1.6;
      color: var(--on-surface-variant);
    }

    .code-preview .keyword {
      color: var(--secondary);
    }

    .code-preview .type {
      color: var(--primary);
    }

    .code-preview .method {
      color: var(--tertiary);
    }

    // Responsive
    @media (max-width: 1200px) {
      .metrics-grid {
        grid-template-columns: repeat(2, 1fr);
      }

      .content-grid {
        grid-template-columns: 1fr;
      }

      .featured-card {
        flex-direction: column;
      }

      .featured-visual {
        min-height: 300px;
      }
    }

    @media (max-width: 768px) {
      .metrics-grid {
        grid-template-columns: 1fr;
      }

      .page-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 16px;
      }

      .page-header h1 {
        font-size: 32px;
      }

      .sync-time {
        text-align: left;
      }

      .featured-content {
        padding: 24px;
      }

      .featured-content h2 {
        font-size: 28px;
      }
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  currentTime = '';
  
  codePreview = `<span class="keyword">public class</span> <span class="type">PortfolioService</span>
{
    <span class="keyword">private readonly</span> <span class="type">AppDbContext</span> _context;
    
    <span class="keyword">public async</span> <span class="type">Task</span><<span class="type">Project</span>> <span class="method">GetByIdAsync</span>(<span class="keyword">int</span> id)
    {
        <span class="keyword">return await</span> _context.Projects
            .Include(p => p.Skills)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}`;

  activities = [
    {
      id: 1,
      type: 'commit',
      icon: 'commit',
      title: 'New commit pushed to <strong>kinetic-core</strong>',
      description: 'Refactored animation engine for v2.4.0',
      time: '14m ago'
    },
    {
      id: 2,
      type: 'inquiry',
      icon: 'person_add',
      title: 'New Project Inquiry: <strong>Nexus Fintech UI</strong>',
      description: 'Estimated budget: $12k - $15k',
      time: '2h ago'
    },
    {
      id: 3,
      type: 'traffic',
      icon: 'visibility',
      title: 'Traffic surge detected from <strong>Dribbble.com</strong>',
      description: 'Spike in portfolio project "Atmosphere JS"',
      time: '5h ago'
    },
    {
      id: 4,
      type: 'warning',
      icon: 'warning',
      title: 'Build failure on branch <strong>hotfix/auth-leak</strong>',
      description: 'CI Pipeline error: unit tests timed out',
      time: 'Yesterday'
    }
  ];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadDashboardStats();
    this.updateTime();
    setInterval(() => this.updateTime(), 1000);
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
}
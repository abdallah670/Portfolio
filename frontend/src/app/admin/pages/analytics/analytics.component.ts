import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { DashboardAnalytics } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-admin-analytics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './analytics.component.html',
  styleUrls: ['analytics.component.scss']
})
export class AnalyticsComponent implements OnInit {
  analytics: DashboardAnalytics | null = null;
  loading = true;
  timeframe: '30 DAYS' | '90 DAYS' | '1 YEAR' = '30 DAYS';

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadAnalytics();
  }

  loadAnalytics(): void {
    const days = this.timeframe === '30 DAYS' ? 30 : this.timeframe === '90 DAYS' ? 90 : 365;
    this.loading = true;
    this.api.getAnalyticsDashboard(days).subscribe({
      next: (data) => {
        this.analytics = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  setTimeframe(tf: '30 DAYS' | '90 DAYS' | '1 YEAR') {
    this.timeframe = tf;
    this.loadAnalytics();
  }

  formatNumber(num: number): string {
    if (num >= 1000000) {
      return (num / 1000000).toFixed(1) + 'M';
    }
    if (num >= 1000) {
      return (num / 1000).toFixed(1) + 'K';
    }
    return num?.toString() || '0';
  }

  getMaxProjectViews(): number {
    if (!this.analytics?.topProjects?.length) return 1;
    return Math.max(...this.analytics.topProjects.map(p => p.views));
  }

  getProjectPercentage(views: number): number {
    const max = this.getMaxProjectViews();
    return max > 0 ? (views / max) * 100 : 0;
  }

  getTrafficPath(): string {
    if (!this.analytics?.trafficTrends?.length) return '';
    const trends = this.analytics.trafficTrends;
    const points = trends.map((t, i) => {
      const x = (i / (trends.length - 1)) * 100;
      const y = 100 - ((t.visitors / this.getMaxVisitors()) * 80 + 10);
      return `${x},${y}`;
    });
    return `M0,100 L${points.join(' L')} L100,100 Z`;
  }

  getTrafficLinePath(): string {
    if (!this.analytics?.trafficTrends?.length) return '';
    const trends = this.analytics.trafficTrends;
    const points = trends.map((t, i) => {
      const x = (i / (trends.length - 1)) * 100;
      const y = 100 - ((t.visitors / this.getMaxVisitors()) * 80 + 10);
      return `${x},${y}`;
    });
    return `M${points.join(' L')}`;
  }

  getMaxVisitors(): number {
    if (!this.analytics?.trafficTrends?.length) return 1;
    return Math.max(...this.analytics.trafficTrends.map(t => t.visitors));
  }
}

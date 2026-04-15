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
    this.loading = true;
    this.api.getAllProjectsAdmin().subscribe({
      next: (data) => {
        this.projects = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getStack(p: Project): string[] {
    try { return JSON.parse(p.stack); } catch { return []; }
  }

  getImageUrl(path: string): string {
    if (!path) return '';
    if (path.startsWith('http')) return path;
    return `http://localhost:5000/${path}`;
  }

  getPublishedCount(): number {
    return this.projects.filter(p => p.status === 'Production').length;
  }

  getDraftCount(): number {
    return this.projects.filter(p => p.status !== 'Production').length;
  }

  getFeaturedCount(): number {
    return this.projects.filter(p => p.isFeatured).length;
  }

  togglePublish(p: Project): void {
    const action = p.status === 'Production'
      ? this.api.unpublishProject(p.id)
      : this.api.publishProject(p.id);
    action.subscribe(() => {
      p.status = p.status === 'Production' ? 'Draft' : 'Production';
    });
  }

  deleteProject(id: number): void {
    if (!confirm('Delete this project?')) return;
    this.api.deleteProject(id).subscribe(() => {
      this.projects = this.projects.filter(proj => proj.id !== id);
    });
  }
}
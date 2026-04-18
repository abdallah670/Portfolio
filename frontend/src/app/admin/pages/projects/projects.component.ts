import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { Project, PaginatedResponse } from '../../../core/models/portfolio.models';
import { SweetAlertService } from '../../../core/services/sweetalert.service';

@Component({
  selector: 'app-admin-projects',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  projects: Project[] = [];
  loading = true;

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  // Modal state
  modalOpen = false;
  editMode = false;
  currentProject: Project | null = null;

  // Form model
  form = {
    title: '',
    year: '',
    category: '',
    description: '',
    stack: [] as string[],
    image: '',
    linkedinUrl: '',
    githubUrl: '',
    liveUrl: '',
    status: 'Production' as 'Production' | 'Draft',
    color: 'blue',
    isFeatured: false,
    displayOrder: 0
  };

  // Stack options for multi-select
  stackOptions = [
    'C#', '.NET', '.NET 9', 'ASP.NET Core', 'Entity Framework',
    'SQL Server', 'Angular', 'Angular 17+', 'NgRx',
    'SignalR', 'Stripe', 'Hangfire', 'Git', 'REST API'
  ];

  // Category options
  categoryOptions = [
    'Web Application',
    'Full-Stack Platform',
    'API',
    'Mobile App',
    'Desktop App',
    'Microservices',
    'Library/Module'
  ];

  // Color options
  colorOptions = [
    { value: 'blue', label: 'Blue' },
    { value: 'emerald', label: 'Emerald' },
    { value: 'cyan', label: 'Cyan' },
    { value: 'pink', label: 'Pink' },
    { value: 'purple', label: 'Purple' }
  ];

  constructor(private api: ApiService, private sweetAlert: SweetAlertService) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.api.getAllProjectsAdmin(this.currentPage, this.pageSize).subscribe({
      next: (response: PaginatedResponse<Project>) => {
        this.projects = response.items;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
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
    // Count from all projects (we only have current page, this will be updated later)
    return this.projects.filter(p => p.status === 'Production').length;
  }

  getDraftCount(): number {
    // Count from all projects (we only have current page, this will be updated later)
    return this.projects.filter(p => p.status !== 'Production').length;
  }

  getFeaturedCount(): number {
    // Count from all projects (we only have current page, this will be updated later)
    return this.projects.filter(p => p.isFeatured).length;
  }

  // Modal operations
  openCreate(): void {
    this.editMode = false;
    this.currentProject = null;
    this.resetForm();
    this.modalOpen = true;
  }

  openEdit(project: Project): void {
    this.editMode = true;
    this.currentProject = project;
    this.form = {
      title: project.title,
      year: project.year,
      category: project.category,
      description: project.description,
      stack: this.getStack(project),
      image: project.image,
      linkedinUrl: project.linkedinUrl,
      githubUrl: project.githubUrl,
      liveUrl: project.liveUrl,
      status: project.status as 'Production' | 'Draft',
      color: project.color,
      isFeatured: project.isFeatured,
      displayOrder: project.displayOrder
    };
    this.modalOpen = true;
  }

  closeModal(): void {
    this.modalOpen = false;
    this.resetForm();
  }

  private resetForm(): void {
    this.form = {
      title: '',
      year: '',
      category: '',
      description: '',
      stack: [],
      image: '',
      linkedinUrl: '',
      githubUrl: '',
      liveUrl: '',
      status: 'Production',
      color: 'blue',
      isFeatured: false,
      displayOrder: 0
    };
  }

  // Stack multi-select
  toggleStackTag(tag: string): void {
    const index = this.form.stack.indexOf(tag);
    if (index > -1) {
      this.form.stack.splice(index, 1);
    } else {
      this.form.stack.push(tag);
    }
  }

  removeStackTag(tag: string): void {
    const index = this.form.stack.indexOf(tag);
    if (index > -1) {
      this.form.stack.splice(index, 1);
    }
  }

  // Image upload
  onImageSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.uploadImage(file);
    }
  }

  private uploadImage(file: File): void {
    const formData = new FormData();
    formData.append('file', file);
    this.api.uploadProjectImage(file).subscribe({
      next: (res) => {
        this.form.image = res.url;
      },
      error: (err) => {
        console.error('Upload failed:', err);
        this.sweetAlert.error('Upload Failed', 'Could not upload image. Please try again.');
      }
    });
  }

  // Save project
  saveProject(): void {
    if (!this.form.title || !this.form.category) {
      this.sweetAlert.error('Required Fields Missing', 'Please fill in Title and Category.');
      return;
    }

    const projectData = {
      ...this.form,
      stack: JSON.stringify(this.form.stack)
    };

    if (this.editMode && this.currentProject) {
      // Update existing
      this.api.updateProject({ ...this.currentProject, ...projectData }).subscribe({
        next: () => {
          this.closeModal();
          this.loadProjects();
          this.sweetAlert.success('Project Updated', 'Project updated successfully.');
        },
        error: (err) => {
          console.error('Update failed:', err);
          this.sweetAlert.error('Update Failed', 'Could not update project. Please try again.');
        }
      });
    } else {
      // Create new
      this.api.createProject(projectData).subscribe({
        next: () => {
          this.closeModal();
          this.loadProjects();
          this.sweetAlert.success('Project Created', 'New project added successfully.');
        },
        error: (err) => {
          console.error('Create failed:', err);
          this.sweetAlert.error('Creation Failed', 'Could not create project. Please try again.');
        }
      });
    }
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
    const project = this.projects.find(p => p.id === id);
    this.sweetAlert.deleteConfirm(project?.title).then((confirmed) => {
      if (confirmed) {
        this.api.deleteProject(id).subscribe({
          next: () => {
            // Reload current page after delete
            this.loadProjects();
            this.sweetAlert.success('Deleted', 'Project deleted successfully.');
          },
          error: (err) => {
            console.error('Delete failed:', err);
            this.sweetAlert.error('Delete Failed', 'Could not delete project.');
          }
        });
      }
    });
  }

  // Pagination Methods
  get paginationStartIndex(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get paginationEndIndex(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadProjects();
    }
  }

  goToNextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadProjects();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.loadProjects();
    }
  }
}
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  exact?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside class="sidebar">
      <!-- Logo -->
      <div class="logo-section">
        <div class="logo-icon">
          <span class="material-symbols-outlined filled">bolt</span>
        </div>
        <div class="logo-text">
          <h1>Kinetic Admin</h1>
          <p>v2.4.0</p>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="nav-section">
        @for (item of navItems; track item.route) {
          <a 
            class="nav-item"
            [class.active]="isActive(item.route, item.exact ?? false)"
            [routerLink]="item.route"
            routerLinkActive="active"
            [routerLinkActiveOptions]="{ exact: item.exact ?? false }"
          >
            <span class="material-symbols-outlined">{{ item.icon }}</span>
            <span class="nav-label">{{ item.label }}</span>
          </a>
        }
      </nav>

      <!-- New Project Button -->
      <div class="action-section">
        <button class="btn-new-project" routerLink="/admin/projects" [queryParams]="{ new: true }">
          <span class="material-symbols-outlined">add</span>
          <span>New Project</span>
        </button>
      </div>

      <!-- Bottom Section -->
      <div class="bottom-section">
        <nav class="secondary-nav">
          <a class="nav-item-small" href="#">
            <span class="material-symbols-outlined">help</span>
            <span>Support</span>
          </a>
          <button class="nav-item-small" (click)="logout()">
            <span class="material-symbols-outlined">logout</span>
            <span>Logout</span>
          </button>
        </nav>

        <!-- User Profile -->
        <div class="user-profile">
          <div class="avatar">
            <span class="initials">AM</span>
          </div>
          <div class="user-info">
            <p class="name">Abdullah Mohammed</p>
            <p class="role">Full-Stack Dev</p>
          </div>
        </div>
      </div>
    </aside>
  `,
  styles: [`
    .sidebar {
      position: fixed;
      left: 0;
      top: 0;
      height: 100vh;
      width: 260px;
      background: var(--surface-container-low);
      border-right: 1px solid var(--outline-variant);
      display: flex;
      flex-direction: column;
      z-index: 100;
      font-family: var(--font-headline);
    }

    .logo-section {
      padding: 24px;
      display: flex;
      align-items: center;
      gap: 12px;
      border-bottom: 1px solid var(--outline-variant);
    }

    .logo-icon {
      width: 40px;
      height: 40px;
      background: linear-gradient(135deg, var(--primary), var(--secondary));
      border-radius: var(--radius-lg);
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .logo-icon .material-symbols-outlined {
      color: var(--on-primary-container);
      font-size: 24px;
    }

    .logo-icon .material-symbols-outlined.filled {
      font-variation-settings: 'FILL' 1, 'wght' 400, 'GRAD' 0, 'opsz' 24;
    }

    .logo-text h1 {
      font-size: 18px;
      font-weight: 700;
      color: var(--secondary);
      text-transform: uppercase;
      letter-spacing: 0.1em;
    }

    .logo-text p {
      font-size: 10px;
      color: var(--on-surface-variant);
      opacity: 0.5;
      text-transform: uppercase;
      letter-spacing: 0.2em;
    }

    .nav-section {
      flex: 1;
      padding: 16px 12px;
      display: flex;
      flex-direction: column;
      gap: 4px;
      overflow-y: auto;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 16px;
      border-radius: var(--radius-lg);
      color: var(--on-surface);
      opacity: 0.6;
      text-decoration: none;
      transition: all 0.2s ease;
      font-weight: 500;
      font-size: 14px;
    }

    .nav-item:hover {
      background: var(--surface-container-high);
      opacity: 1;
      color: var(--secondary);
    }

    .nav-item.active {
      background: var(--surface-container-high);
      color: var(--secondary);
      border-left: 2px solid var(--secondary);
      opacity: 1;
      font-weight: 700;
    }

    .nav-item .material-symbols-outlined {
      font-size: 20px;
    }

    .action-section {
      padding: 0 16px 16px;
    }

    .btn-new-project {
      width: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      padding: 12px;
      background: linear-gradient(135deg, var(--primary), var(--primary-container));
      color: var(--on-primary-fixed);
      border-radius: var(--radius-lg);
      font-weight: 700;
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      cursor: pointer;
      transition: all 0.2s ease;
      box-shadow: 0 4px 20px rgba(155, 168, 255, 0.2);
    }

    .btn-new-project:hover {
      opacity: 0.9;
      transform: translateY(-1px);
    }

    .btn-new-project .material-symbols-outlined {
      font-size: 16px;
    }

    .bottom-section {
      padding: 16px;
      border-top: 1px solid var(--outline-variant);
    }

    .secondary-nav {
      display: flex;
      flex-direction: column;
      gap: 4px;
      margin-bottom: 16px;
    }

    .nav-item-small {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 8px 12px;
      border-radius: var(--radius-md);
      color: var(--on-surface);
      opacity: 0.6;
      text-decoration: none;
      transition: all 0.2s ease;
      font-size: 13px;
      background: none;
      border: none;
      cursor: pointer;
      width: 100%;
      text-align: left;
    }

    .nav-item-small:hover {
      background: var(--surface-container-high);
      opacity: 1;
      color: var(--secondary);
    }

    .nav-item-small .material-symbols-outlined {
      font-size: 18px;
    }

    .user-profile {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px;
      background: var(--surface-container);
      border-radius: var(--radius-lg);
    }

    .avatar {
      width: 40px;
      height: 40px;
      background: var(--surface-container-highest);
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      border: 2px solid var(--outline-variant);
    }

    .avatar .initials {
      font-size: 14px;
      font-weight: 700;
      color: var(--primary);
    }

    .user-info .name {
      font-size: 13px;
      font-weight: 700;
      color: var(--on-surface);
      margin-bottom: 2px;
    }

    .user-info .role {
      font-size: 11px;
      color: var(--on-surface-variant);
      opacity: 0.6;
    }
  `]
})
export class SidebarComponent {
  navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/admin/dashboard', exact: true },
    { label: 'Projects', icon: 'folder_special', route: '/admin/projects', exact: false },
    { label: 'Analytics', icon: 'monitoring', route: '/admin/analytics', exact: false },
    { label: 'Messages', icon: 'mail', route: '/admin/messages', exact: false },
    { label: 'Settings', icon: 'settings', route: '/admin/settings', exact: false }
  ];

  constructor(private authService: AuthService, private router: Router) {}

  isActive(route: string, exact?: boolean): boolean {
    if (exact) {
      return this.router.url === route;
    }
    return this.router.url.startsWith(route);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
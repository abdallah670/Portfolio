import { Component, Inject, OnInit, Input, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="admin-header">
      <div class="header-left">
        <a routerLink="/admin/dashboard" class="logo">Admin</a>
      </div>
      <nav class="header-nav">
        <a routerLink="/admin/dashboard" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link"><span class="material-symbols-outlined">dashboard</span><span>Dashboard</span></a>
        <a routerLink="/admin/projects" routerLinkActive="active" class="nav-link"><span class="material-symbols-outlined">folder_special</span><span>Projects</span></a>
        <a routerLink="/admin/messages" routerLinkActive="active" class="nav-link"><span class="material-symbols-outlined">mail</span><span>Messages</span></a>
        <a routerLink="/admin/settings" routerLinkActive="active" class="nav-link"><span class="material-symbols-outlined">settings</span><span>Settings</span></a>
      </nav>
      <div class="header-right">
        <button class="theme-toggle" (click)="toggleTheme()">{{ isDarkTheme ? '☀' : '☾' }}</button>
        <div class="user-menu" (click)="userMenuOpen = !userMenuOpen">
          @if (profileImage) {
            <img [src]="profileImageUrl" class="user-avatar-img" alt="Profile">
          } @else {
            <div class="user-avatar">AM</div>
          }
          <span class="user-name">Abdullah</span>
          <span class="material-symbols-outlined">expand_more</span>
        </div>
        @if (userMenuOpen) {
          <div class="user-dropdown">
            <div class="dropdown-user-info">
              @if (profileImage) {
                <img [src]="profileImageUrl" class="dropdown-avatar-img" alt="Profile">
              } @else {
                <div class="dropdown-avatar">AM</div>
              }
              <div><p class="name">Abdullah</p><p class="role"></p></div>
            </div>
            <div class="dropdown-divider"></div>
            <button class="dropdown-item" (click)="logout()"><span class="material-symbols-outlined">logout</span><span>Logout</span></button>
          </div>
        }
      </div>
    </header>
  `,
  styles: [`
    .admin-header { position: fixed; top: 0; left: 0; right: 0; height: 64px; background: var(--surface-container-low); border-bottom: 1px solid var(--outline-variant); display: flex; align-items: center; padding: 0 24px; z-index: 100; }
    .logo { font-size: 20px; font-weight: 700; color: var(--primary); text-decoration: none; }
    .header-nav { display: flex; gap: 8px; margin-left: 48px; flex: 1; }
    .nav-link { display: flex; align-items: center; gap: 8px; padding: 8px 16px; border-radius: var(--radius-md); color: var(--on-surface); text-decoration: none; font-size: 14px; font-weight: 500; opacity: 0.7; }
    .nav-link:hover, .nav-link.active { background: var(--surface-container-high); opacity: 1; color: var(--primary); }
    .nav-link .material-symbols-outlined { font-size: 20px; }
    .header-right { display: flex; align-items: center; gap: 16px; position: relative; }
    .theme-toggle { width: 36px; height: 36px; border-radius: 50%; border: 1px solid var(--outline-variant); background: transparent; color: var(--on-surface); cursor: pointer; }
    .user-menu { display: flex; align-items: center; gap: 12px; padding: 6px; border-radius: var(--radius-lg); cursor: pointer; }
    .user-menu:hover { background: var(--surface-container-high); }
    .user-avatar { width: 32px; height: 32px; background: linear-gradient(135deg, var(--primary), var(--secondary)); border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 12px; font-weight: 700; color: var(--on-primary); }
    .user-avatar-img { width: 32px; height: 32px; border-radius: 50%; object-fit: cover; }
    .user-name { font-size: 14px; font-weight: 500; color: var(--on-surface); }
    .dropdown-icon { font-size: 20px; color: var(--on-surface-variant); }
    .user-dropdown { position: absolute; top: calc(100% + 8px); right: 0; width: 240px; background: var(--surface-container); border: 1px solid var(--outline-variant); border-radius: var(--radius-lg); padding: 8px; }
    .dropdown-user-info { display: flex; align-items: center; gap: 12px; padding: 12px; }
    .dropdown-avatar { width: 40px; height: 40px; background: linear-gradient(135deg, var(--primary), var(--secondary)); border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 14px; font-weight: 700; color: var(--on-primary); }
    .dropdown-avatar-img { width: 40px; height: 40px; border-radius: 50%; object-fit: cover; }
    .dropdown-user-info .name { font-size: 14px; font-weight: 600; color: var(--on-surface); }
    .dropdown-user-info .role { font-size: 12px; color: var(--on-surface-variant); }
    .dropdown-divider { height: 1px; background: var(--outline-variant); margin: 8px 0; }
    .dropdown-item { display: flex; align-items: center; gap: 12px; width: 100%; padding: 10px 12px; border: none; background: none; border-radius: var(--radius-md); color: var(--on-surface); font-size: 14px; cursor: pointer; }
    .dropdown-item:hover { background: var(--surface-container-high); color: var(--error); }
    @media (max-width: 768px) { .header-nav { display: none; } .user-name { display: none; } }
  `]
})
export class HeaderComponent implements OnInit {
  @Input() profileImage: string = '';
  isDarkTheme = true;
  userMenuOpen = false;
  private isBrowser: boolean;
  constructor(@Inject(PLATFORM_ID) private platformId: Object, private authService: AuthService, private router: Router) { this.isBrowser = isPlatformBrowser(this.platformId); }
  ngOnInit(): void { if (this.isBrowser) { this.isDarkTheme = localStorage.getItem('am-theme') !== 'light'; this.applyTheme(); } }

  get profileImageUrl(): string {
    if (!this.profileImage) return '';
    if (this.profileImage.startsWith('http')) return this.profileImage;
    return `http://localhost:5000/${this.profileImage}`;
  }
  toggleTheme(): void { this.isDarkTheme = !this.isDarkTheme; this.applyTheme(); }
  private applyTheme(): void { if (this.isBrowser) { document.documentElement.setAttribute('data-theme', this.isDarkTheme ? 'dark' : 'light'); localStorage.setItem('am-theme', this.isDarkTheme ? 'dark' : 'light'); } }
  logout(): void { this.authService.logout(); this.router.navigate(['/login']); }
}

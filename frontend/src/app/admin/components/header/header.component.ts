import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <header class="header">
      <div class="header-left">
        <!-- Search -->
        <div class="search-box">
          <span class="material-symbols-outlined search-icon">search</span>
          <input 
            type="text" 
            placeholder="Search archive..."
            [(ngModel)]="searchQuery"
            (keyup.enter)="onSearch()"
          />
        </div>

        <!-- Nav Tabs -->
        <nav class="nav-tabs">
          <a class="nav-tab active" href="#">Overview</a>
          <a class="nav-tab" href="#">History</a>
          <a class="nav-tab" href="#">Reports</a>
        </nav>
      </div>

      <div class="header-right">
        <!-- Notifications -->
        <button class="icon-btn">
          <span class="material-symbols-outlined">notifications</span>
          <span class="badge" *ngIf="notificationCount > 0">{{ notificationCount }}</span>
        </button>

        <!-- Apps Menu -->
        <button class="icon-btn">
          <span class="material-symbols-outlined">apps</span>
        </button>

        <div class="divider"></div>

        <!-- User -->
        <div class="user-menu">
          <span class="user-label">THE KINETIC<br/>ARCHIVE</span>
          <div class="avatar">
            <span>AM</span>
          </div>
        </div>
      </div>
    </header>
  `,
  styles: [`
    .header {
      position: fixed;
      top: 0;
      left: 260px;
      right: 0;
      height: 64px;
      background: rgba(6, 14, 32, 0.8);
      backdrop-filter: blur(20px);
      border-bottom: 1px solid var(--outline-variant);
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 32px;
      z-index: 90;
    }

    .header-left {
      display: flex;
      align-items: center;
      gap: 32px;
    }

    .search-box {
      position: relative;
      width: 320px;
    }

    .search-box input {
      width: 100%;
      background: var(--surface-container-low);
      border: 1px solid transparent;
      border-radius: var(--radius-full);
      padding: 10px 16px 10px 40px;
      color: var(--on-surface);
      font-size: 13px;
      font-family: var(--font-body);
      transition: all 0.2s ease;
    }

    .search-box input::placeholder {
      color: var(--on-surface-variant);
      opacity: 0.5;
    }

    .search-box input:focus {
      outline: none;
      border-color: var(--secondary);
      box-shadow: 0 0 0 2px rgba(0, 227, 253, 0.1);
    }

    .search-icon {
      position: absolute;
      left: 12px;
      top: 50%;
      transform: translateY(-50%);
      color: var(--on-surface-variant);
      font-size: 18px;
    }

    .nav-tabs {
      display: flex;
      align-items: center;
      gap: 24px;
    }

    .nav-tab {
      font-family: var(--font-headline);
      font-size: 13px;
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      color: var(--on-surface);
      opacity: 0.7;
      text-decoration: none;
      padding-bottom: 4px;
      border-bottom: 2px solid transparent;
      transition: all 0.2s ease;
    }

    .nav-tab:hover {
      opacity: 1;
      color: var(--secondary);
    }

    .nav-tab.active {
      opacity: 1;
      color: var(--secondary);
      border-bottom-color: var(--secondary);
    }

    .header-right {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .icon-btn {
      position: relative;
      width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      color: var(--on-surface-variant);
      transition: all 0.2s ease;
      cursor: pointer;
    }

    .icon-btn:hover {
      background: var(--surface-container-high);
      color: var(--secondary);
    }

    .icon-btn .material-symbols-outlined {
      font-size: 20px;
    }

    .badge {
      position: absolute;
      top: 4px;
      right: 4px;
      width: 16px;
      height: 16px;
      background: var(--error);
      color: var(--on-error);
      font-size: 10px;
      font-weight: 700;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .divider {
      width: 1px;
      height: 24px;
      background: var(--outline-variant);
      opacity: 0.3;
      margin: 0 8px;
    }

    .user-menu {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .user-label {
      font-family: var(--font-headline);
      font-size: 10px;
      font-weight: 700;
      color: var(--primary);
      text-transform: uppercase;
      letter-spacing: 0.05em;
      line-height: 1.2;
      text-align: right;
    }

    .avatar {
      width: 36px;
      height: 36px;
      background: var(--surface-container-highest);
      border-radius: var(--radius-lg);
      display: flex;
      align-items: center;
      justify-content: center;
      border: 1px solid var(--outline-variant);
      font-size: 12px;
      font-weight: 700;
      color: var(--primary);
    }

    @media (max-width: 1024px) {
      .nav-tabs {
        display: none;
      }

      .search-box {
        width: 240px;
      }
    }
  `]
})
export class HeaderComponent {
  searchQuery = '';
  notificationCount = 3;

  onSearch(): void {
    if (this.searchQuery.trim()) {
      console.log('Searching for:', this.searchQuery);
    }
  }
}

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { HeaderComponent } from './components/header/header.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SidebarComponent, HeaderComponent],
  template: `
    <div class="admin-layout">
      <app-sidebar />
      <app-header />
      <main class="main-content">
        <div class="content-wrapper">
          <router-outlet />
        </div>
      </main>
    </div>
  `,
  styles: [`
    .admin-layout {
      min-height: 100vh;
      background: var(--background);
    }

    .main-content {
      margin-left: 260px;
      padding-top: 64px;
      min-height: 100vh;
    }

    .content-wrapper {
      padding: 32px;
      max-width: 1600px;
      margin: 0 auto;
    }

    @media (max-width: 768px) {
      .main-content {
        margin-left: 0;
      }

      .content-wrapper {
        padding: 16px;
      }
    }
  `]
})
export class AdminLayoutComponent {}
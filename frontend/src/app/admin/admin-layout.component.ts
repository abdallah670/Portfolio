import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { ApiService } from '../core/services/api.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HeaderComponent],
  template: `
    <div class="admin-layout">
      <app-header [profileImage]="profileImage" />
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
      padding-top: 64px;
      min-height: 100vh;
    }

    .content-wrapper {
      padding: 32px;
      max-width: 1600px;
      margin: 0 auto;
    }

    @media (max-width: 768px) {
      .content-wrapper {
        padding: 16px;
      }
    }
  `]
})
export class AdminLayoutComponent implements OnInit {
  profileImage = '';
  private isBrowser: boolean;
  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private apiService: ApiService
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      const saved = localStorage.getItem('am-profile-image');
      if (saved) {
        this.profileImage = saved;
      } else {
        this.apiService.getProfileImage().subscribe({
          next: (image) => {
            if (image) {
              this.profileImage = image;
              localStorage.setItem('am-profile-image', this.profileImage);
            }
          },
          error: () => {}
        });
      }
    }
  }
}
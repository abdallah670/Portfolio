import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { ApiService } from '../core/services/api.service';
import { UserService } from '../core/services/user.service';

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
      display: flex;
      flex-direction: column;
      min-height: calc(100vh - 64px);
      padding-top: 64px;
    }

    .content-wrapper {
      width: 100%;
      max-width: 1600px;
      margin: 0 auto;
      padding-top: 24px;
    }
  `]
})
export class AdminLayoutComponent implements OnInit {
  profileImage = '';
  private isBrowser: boolean;
  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private apiService: ApiService,
    private userService: UserService
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      // Initialize from localStorage first
      const cachedImage = localStorage.getItem('am-profile-image');
      if (cachedImage) {
        this.profileImage = cachedImage;
        this.userService.setProfileImage(cachedImage);
      }
      
      // Subscribe to profile image changes
      this.userService.profileImage$.subscribe(image => {
        if (image) {
          this.profileImage = image;
        }
      });
      
      // Always fetch from API to ensure we have latest data
      this.apiService.getProfileImage().subscribe({
        next: (image) => {
          if (image) {
            this.userService.setProfileImage(image);
          }
        },
        error: () => {}
      });
    }
  }
}
import { Component, Inject, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <header class="header">
      <div class="navbar-actions">
        <button class="theme-toggle" (click)="toggleTheme()" aria-label="Toggle theme">
          <span>{{ isDarkTheme ? '☀' : '☾' }}</span>
        </button>
        <button class="hamburger" [class.open]="mobileMenuOpen" (click)="toggleMobileMenu()" aria-expanded="mobileMenuOpen" aria-label="Open menu">
          <span></span><span></span><span></span>
        </button>
      </div>
    </header>
  `,
  styles: [`
    .navbar-actions { 
  display: flex; 
  align-items: center; 
  gap: 0.75rem; 
}

.theme-toggle {
  width: 34px; 
  height: 34px; 
  border-radius: 50%;
  border: 1px solid var(--border); 
  background: transparent;
  color: var(--text-secondary); 
  cursor: pointer;
  display: flex; 
  align-items: center; 
  justify-content: center;
  font-size: 0.85rem; 
  transition: all var(--transition);
}

.theme-toggle:hover { 
  border-color: var(--accent); 
  color: var(--accent); 
  background: var(--accent-dim); 
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
export class HeaderComponent implements OnInit {
  isScrolled = false;
  mobileMenuOpen = false;
  isDarkTheme = true;
  private isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      // Check for saved theme preference
      const savedTheme = localStorage.getItem('am-theme');
      if (savedTheme) {
        this.isDarkTheme = savedTheme === 'dark';
      } else {
        this.isDarkTheme = true;
      }
      this.applyTheme();

      // Listen for scroll events
      window.addEventListener('scroll', () => {
        this.isScrolled = window.scrollY > 20;
      }, { passive: true });
    }
  }

  toggleTheme(): void {
    this.isDarkTheme = !this.isDarkTheme;
    this.applyTheme();
  }

  private applyTheme(): void {
    if (this.isBrowser) {
      const theme = this.isDarkTheme ? 'dark' : 'light';
      document.documentElement.setAttribute('data-theme', theme);
      localStorage.setItem('am-theme', theme);
    }
  }

}

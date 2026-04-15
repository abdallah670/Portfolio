import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="navbar" id="navbar" [class.scrolled]="isScrolled">
      <a routerLink="/" class="navbar-logo">Abdullah<span>.</span></a>
      
      <ul class="navbar-nav">
        <li><a routerLink="/" fragment="about" (click)="scrollTo('about')">About</a></li>
        <li><a routerLink="/" fragment="skills" (click)="scrollTo('skills')">Skills</a></li>
        <li><a routerLink="/" fragment="projects" (click)="scrollTo('projects')">Projects</a></li>
        <li><a routerLink="/" fragment="journey" (click)="scrollTo('journey')">Journey</a></li>
        <li><a routerLink="/contact">Contact</a></li>
      </ul>
      
      <div class="navbar-actions">
        <button class="theme-toggle" (click)="toggleTheme()" aria-label="Toggle theme">
          <span>{{ isDarkTheme ? '☀' : '☾' }}</span>
        </button>
        <button class="hamburger" [class.open]="mobileMenuOpen" (click)="toggleMobileMenu()" aria-expanded="mobileMenuOpen" aria-label="Open menu">
          <span></span><span></span><span></span>
        </button>
      </div>
    </nav>

    <div class="mobile-menu" [class.open]="mobileMenuOpen">
      <a routerLink="/" fragment="about" (click)="closeMobileMenu(); scrollTo('about')">About</a>
      <a routerLink="/" fragment="skills" (click)="closeMobileMenu(); scrollTo('skills')">Skills</a>
      <a routerLink="/" fragment="projects" (click)="closeMobileMenu(); scrollTo('projects')">Projects</a>
      <a routerLink="/" fragment="journey" (click)="closeMobileMenu(); scrollTo('journey')">Journey</a>
      <a routerLink="/contact" (click)="closeMobileMenu()">Contact</a>
    </div>
  `,
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
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

  toggleMobileMenu(): void {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen = false;
  }

  scrollTo(elementId: string): void {
    if (this.isBrowser) {
      const element = document.getElementById(elementId);
      if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
      }
    }
  }
}

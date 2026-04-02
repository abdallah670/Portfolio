import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';

import { HeroComponent } from './components/hero/hero.component';
import { AboutComponent } from './components/about/about.component';
import { SkillsComponent } from './components/skills/skills.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { CtaComponent } from './components/cta/cta.component';

import { ApiService } from '../../../core/services/api.service';
import { PortfolioConfig } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    NavbarComponent,
    FooterComponent,
    HeroComponent,
    AboutComponent,
    SkillsComponent,
    ProjectsComponent,
    CtaComponent
  ],
  template: `
    <div class="home-page">
      <app-navbar></app-navbar>

      @if (loading) {
        <div class="loading-state">
          <div class="loading-spinner"></div>
        </div>
      }

      @if (!loading && config) {
        <main class="main-content">
          <app-home-hero [hero]="config.hero"></app-home-hero>
          <app-home-about [about]="config.about"></app-home-about>
          <app-home-skills [skills]="config.skills"></app-home-skills>
          <app-home-projects
            [featuredProjects]="config.featuredProjects"
            [moreProjects]="config.moreProjects">
          </app-home-projects>
          <app-home-cta></app-home-cta>
        </main>
      }

      <app-footer></app-footer>
    </div>
  `,
  styles: [`
    .home-page {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      background: var(--background);
      color: var(--on-surface);
    }

    .main-content {
      flex: 1;
    }

    .loading-state {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 60vh;
    }

    .loading-spinner {
      width: 48px;
      height: 48px;
      border: 3px solid var(--outline-variant);
      border-top-color: var(--primary);
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class HomeComponent implements OnInit {
  config: PortfolioConfig | null = null;
  loading = true;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.api.getPortfolioConfig().subscribe({
      next: (data) => { this.config = data; this.loading = false; },
      error: (err) => { console.error('Failed to load portfolio config:', err); this.loading = false; }
    });
  }
}
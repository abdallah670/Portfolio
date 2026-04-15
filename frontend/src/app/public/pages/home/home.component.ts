import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { PortfolioConfig } from '../../../core/models/portfolio.models';

import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';

import { HeroComponent } from './components/hero/hero.component';
import { CvComponent } from './components/cv/cv.component';
import { AboutComponent } from './components/about/about.component';
import { SkillsComponent } from './components/skills/skills.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { JourneyComponent } from './components/journey/journey.component';
import { CtaComponent } from './components/cta/cta.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    NavbarComponent,
    FooterComponent,
    HeroComponent,
    CvComponent,
    AboutComponent,
    SkillsComponent,
    ProjectsComponent,
    JourneyComponent,
    CtaComponent
  ],
  templateUrl: './home.component.html',
  styles: [`
    .home-page {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      background: var(--bg-primary);
      color: var(--text-primary);
    }

    .main-content {
      flex: 1;
    }
  `]
})
export class HomeComponent implements OnInit {
  config: PortfolioConfig | null = null;
  loading = true;
  error = '';

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadConfig();
  }

  loadConfig(): void {
    this.loading = true;
    this.error = '';
    this.api.getPortfolioConfig().subscribe({
      next: (data) => {
        this.config = data;
        this.loading = false;
        setTimeout(() => {
          if (typeof (window as any).initPortfolioJS === 'function') {
            (window as any).initPortfolioJS();
          }
        }, 50);
      },
      error: (err) => {
        this.error = 'Failed to load portfolio data.';
        this.loading = false;
      }
    });
  }
}

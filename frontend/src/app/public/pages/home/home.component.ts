import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { PortfolioConfig } from '../../../core/models/portfolio.models';

import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';

import { HeroComponent } from './components/hero/hero.component';
import { AboutComponent } from './components/about/about.component';
import { SkillsComponent } from './components/skills/skills.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { JourneyComponent } from './components/journey/journey.component';
import { CtaComponent } from './components/cta/cta.component';
import { CvComponent } from './components/cv/cv.component';

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
    JourneyComponent,
    CtaComponent,
    CvComponent
  ],
  templateUrl: 'home.component.html',
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
      padding-top: 62px;
    }
  `]
})
export class HomeComponent implements OnInit {
  config: PortfolioConfig | null = null;
  loading = true;
  error = '';
  private isBrowser: boolean;

  constructor(
    private api: ApiService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    this.api.getPortfolioConfig().subscribe({
      next: (data) => {
        this.config = data;
        this.loading = false;
        
        if (this.isBrowser) {
          setTimeout(() => this.initScrollReveal(), 100);
          setTimeout(() => this.initSkillBars(), 100);
          setTimeout(() => this.initMagneticButtons(), 100);
          setTimeout(() => this.initRippleEffect(), 100);
        }
      },
      error: (err) => {
        console.error('Failed to load portfolio data:', err);
        this.error = 'Failed to load portfolio data.';
        this.loading = false;
      }
    });
  }

  private initScrollReveal() {
    if ('IntersectionObserver' in window) {
      const revealObs = new IntersectionObserver((entries) => {
        entries.forEach(e => {
          if (e.isIntersecting) {
            e.target.classList.add('visible');
            revealObs.unobserve(e.target);
          }
        });
      }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

      document.querySelectorAll('.reveal, .reveal-left, .reveal-right, .reveal-scale').forEach(el => {
        revealObs.observe(el);
      });
    }
  }

  private initSkillBars() {
    if ('IntersectionObserver' in window) {
      const barObs = new IntersectionObserver((entries) => {
        entries.forEach(e => {
          if (e.isIntersecting) {
            e.target.classList.add('bar-visible');
            barObs.unobserve(e.target);
          }
        });
      }, { threshold: 0.4 });

      document.querySelectorAll('.skill-row').forEach(el => {
        barObs.observe(el);
      });
    }
  }

  private initMagneticButtons() {
    document.querySelectorAll('.mag-btn').forEach(btn => {
      btn.addEventListener('mousemove', (e: Event) => {
        const target = btn as HTMLElement;
        const rect = target.getBoundingClientRect();
        const x = (e as MouseEvent).clientX - rect.left - rect.width / 2;
        const y = (e as MouseEvent).clientY - rect.top - rect.height / 2;
        target.style.setProperty('--mag-x', `${x * 0.15}px`);
        target.style.setProperty('--mag-y', `${y * 0.15}px`);
      });
      btn.addEventListener('mouseleave', () => {
        const target = btn as HTMLElement;
        target.style.setProperty('--mag-x', '0px');
        target.style.setProperty('--mag-y', '0px');
      });
    });
  }

  private initRippleEffect() {
    document.querySelectorAll('.btn').forEach(btn => {
      btn.addEventListener('click', (e: Event) => {
        const target = btn as HTMLElement;
        const rect = target.getBoundingClientRect();
        const r = document.createElement('span');
        r.className = 'ripple';
        const size = Math.max(rect.width, rect.height);
        r.style.cssText = `width:${size}px;height:${size}px;left:${(e as MouseEvent).clientX - rect.left - size/2}px;top:${(e as MouseEvent).clientY - rect.top - size/2}px`;
        target.appendChild(r);
        setTimeout(() => r.remove(), 700);
      });
    });
  }
}

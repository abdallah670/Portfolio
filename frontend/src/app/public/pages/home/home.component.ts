import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';

import { HeroComponent } from './components/hero/hero.component';
import { AboutComponent } from './components/about/about.component';
import { SkillsComponent } from './components/skills/skills.component';
import { ProjectsComponent } from './components/projects/projects.component';
import { CtaComponent } from './components/cta/cta.component';

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

      <main class="main-content">
        <app-home-hero></app-home-hero>
        <app-home-about></app-home-about>
        <app-home-skills></app-home-skills>
        <app-home-projects></app-home-projects>
        <app-home-cta></app-home-cta>
      </main>

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
  `]
})
export class HomeComponent {}
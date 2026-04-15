import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { SkillCategoryConfig } from '../../../../../core/models/portfolio.models';

interface Skill {
  name: string;
  percent: number;
}

interface SkillCategory {
  title: string;
  dotColor: string;
  skills: Skill[];
}

@Component({
  selector: 'app-home-skills',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './skills.component.html',
  styleUrls: ['./skills.component.scss']
})
export class SkillsComponent implements OnInit {
  @Input() skills: SkillCategoryConfig[] = [];

  description = "I don't just learn syntax; I focus on the underlying architecture. My stack is carefully chosen to build enterprise-grade, scalable backend systems.";

  // Default categories as fallback
  private defaultCategories: SkillCategory[] = [
    {
      title: 'Backend Core',
        dotColor: '#22d3ee',
      skills: [
        { name: 'C#', percent: 90 },
        { name: '.NET', percent: 85 },
        { name: 'Entity Framework', percent: 85 },
        { name: 'Identity & Auth', percent: 80 }
      ]
    },
    {
      title: 'Data & Architecture',
      dotColor: '#22d3ee',
      skills: [
        { name: 'SQL Server', percent: 85 },
        { name: 'Database Design', percent: 80 },
        { name: 'Layered Architecture', percent: 85 },
        { name: 'Clean Code principles', percent: 80 }
      ]
    },
    {
      title: 'Frontend & Dev',
       dotColor: '#22d3ee',
      skills: [
        { name: 'Angular', percent: 75 },
        { name: 'TypeScript', percent: 75 },
        { name: 'Git & Source Control', percent: 85 },
        { name: 'System Debugging', percent: 80 }
      ]
    }
  ];

  barsVisible = false;
  private isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      this.initScrollReveal();
      this.initSkillBars();
    }
  }

  private initScrollReveal(): void {
    if ('IntersectionObserver' in window) {
      const revealObs = new IntersectionObserver((entries) => {
        entries.forEach(e => {
          if (e.isIntersecting) {
            e.target.classList.add('visible');
            revealObs.unobserve(e.target);
          }
        });
      }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

      setTimeout(() => {
        document.querySelectorAll('#skills .reveal').forEach(el => revealObs.observe(el));
      }, 100);
    }
  }

  private initSkillBars(): void {
    if ('IntersectionObserver' in window) {
      const barObs = new IntersectionObserver((entries) => {
        entries.forEach(e => {
          if (e.isIntersecting) {
            this.barsVisible = true;
            barObs.unobserve(e.target);
          }
        });
      }, { threshold: 0.4 });

      setTimeout(() => {
        const skillsSection = document.getElementById('skills');
        if (skillsSection) {
          barObs.observe(skillsSection);
        }
      }, 100);
    }
  }

  get categories(): SkillCategory[] {
    if (this.skills?.length) {
      return this.skills.map(cat => ({
        title: cat.title,
        dotColor: this.getColorForCategory(cat.color),
        skills: cat.skills.map(s => ({ name: s.name, percent: s.level }))
      }));
    }
    return this.defaultCategories;
  }

  private getColorForCategory(color: string): string {
    const colorMap: Record<string, string> = {
      'emerald': '#10b981',
      'cyan': '#22d3ee',
      'blue': '#3b82f6',
      'purple': '#a78bfa',
      'orange': '#f97316',
      'red': '#ef4444',
      'pink': '#ec4899'
    };
    return colorMap[color] || '#3b82f6';
  }
}

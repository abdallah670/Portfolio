import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { AboutConfig, AboutCardConfig, ValueConfig } from '../../../../../core/models/portfolio.models';

interface AboutParagraph {
  html: string;
}

interface AboutCard {
  icon: string;
  title: string;
  sub: string;
}

interface Value {
  title: string;
  desc: string;
}

@Component({
  selector: 'app-home-about',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {
  @Input() about?: AboutConfig;

  aboutCards: AboutCard[] = [
    { icon: '🔷', title: '.NET Developer', sub: 'C#, ADO.NET, Windows Services' },
    { icon: '🗄', title: 'Database Specialist', sub: 'SQL Server, Query Optimization' },
    { icon: '🧱', title: 'System Design', sub: 'Layered Architecture, Clean Code' },
    { icon: '🧩', title: 'Problem Solver', sub: 'Real-world system implementation' }
  ];

  aboutParagraphs = [
    "I'm <strong>Abdullah Mohammed</strong> — a backend-focused <strong>.NET developer</strong> from Cairo, Egypt, studying at the Faculty of Computers and Information. I specialize in building <strong>scalable, production-ready systems</strong> using C#, SQL Server, and clean architecture patterns.",
    "My focus is on the backend — designing data layers, handling business logic, and structuring code that <strong>stays maintainable as systems grow</strong>. I also integrate Angular frontends to bring those systems to life end-to-end."
  ];

  funFact = "I enjoy turning complex system requirements into clean, structured code.";
  subtitleHtml = "A backend developer focused on<br /><em style=\"font-style:italic;color:var(--accent)\">reliable, scalable systems.</em>";

  private isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      this.initScrollReveal();
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
        document.querySelectorAll('#about .reveal').forEach(el => revealObs.observe(el));
      }, 100);
    }
  }

  get cards(): AboutCard[] {
    if (this.about?.cards?.length) {
      // API provides cards, but doesn't have an icon field. We auto-assign based on index or fallback generic.
      const fallbackIcons = ['🔷', '🗄', '🧱', '🧩'];
      return this.about.cards.map((c, i) => ({
        icon: fallbackIcons[i % fallbackIcons.length],
        title: c.title,
        sub: c.subtitle
      }));
    }
    return this.aboutCards;
  }

  get displaySubtitle(): string {
    return this.about?.subtitle || "A backend developer focused on building reliable and scalable systems.";
  }

  get displayFunFact(): string {
    return this.about?.funFact || this.funFact;
  }
}

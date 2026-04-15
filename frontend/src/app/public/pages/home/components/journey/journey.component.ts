import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { JourneyItemConfig } from '../../../../../core/models/portfolio.models';

interface TimelineItem {
  period: string;
  title: string;
  org: string;
  desc: string;
}

@Component({
  selector: 'app-home-journey',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './journey.component.html',
  styleUrls: ['./journey.component.scss']
})
export class JourneyComponent implements OnInit {
  @Input() journey: JourneyItemConfig[] = [];

  timelineItems: TimelineItem[] = [
    {
      period: '2026 - Present',
      title: 'Freelance Full-Stack Developer',
      org: 'Independent',
      desc: 'Taking on complex freelancing projects focusing on .NET backends, system optimizations, and full-stack solutions.'
    },
    {
      period: '2023 - Present',
      title: 'BSc. Computer Science',
      org: 'Faculty of Computers & Information (FCI)',
      desc: 'Studying core computer science concepts, data structures, algorithms, and advanced software engineering principles.'
    },
    {
      period: '2024 - 2025',
      title: 'Backend Specialization',
      org: 'Self-Directed Learning',
      desc: 'Deep dive into C#, .NET ecosystem, SQL Server, design patterns, and enterprise-level system architecture.'
    }
  ];

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
        document.querySelectorAll('#journey .reveal').forEach(el => revealObs.observe(el));
      }, 100);
    }
  }

  get items(): TimelineItem[] {
    if (this.journey?.length) {
      return this.journey.map(j => ({
        period: j.period,
        title: j.title,
        org: j.org,
        desc: j.description
      }));
    }
    return this.timelineItems;
  }
}
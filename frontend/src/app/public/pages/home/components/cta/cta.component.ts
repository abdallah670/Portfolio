import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { ContactConfig, SocialLinkConfig } from '../../../../../core/models/portfolio.models';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home-cta',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cta.component.html',
  styleUrls: ['./cta.component.scss']
})
export class CtaComponent implements OnInit {
  @Input() contact?: ContactConfig;
  @Input() socials: SocialLinkConfig[] = [];

  private isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) private platformId: Object, private router: Router) {
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
        document.querySelectorAll('#cta .reveal').forEach(el => revealObs.observe(el));
      }, 100);
    }
  }

  openContactForm(): void {
    this.router.navigate(['/contact']);
  }

  scrollTo(elementId: string, event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
      
      // Final nuclear fix: lock button completely - bypass Angular change detection
      const button = event.currentTarget as HTMLElement;
      button.classList.add('clicked');
      setTimeout(() => button.classList.remove('clicked'), 250);
    }
    if (this.isBrowser) {
      // Critical fix: defer scrolling to next event loop after click processing finishes
      // This completely avoids the browser layout bug during smooth scroll anchor navigation
      setTimeout(() => {
        const element = document.getElementById(elementId);
        if (element) {
          element.scrollIntoView({ behavior: 'smooth' });
        }
      }, 0);
    }
  }
}

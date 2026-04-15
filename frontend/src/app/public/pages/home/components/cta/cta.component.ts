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
}

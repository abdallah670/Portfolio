import { Component, Inject, PLATFORM_ID, OnDestroy, OnInit } from '@angular/core';
import { isPlatformBrowser, CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  template: `
    <router-outlet />
    <button class="back-to-top" [class.visible]="showBackToTop" (click)="scrollToTop()" aria-label="Back to top">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
        <polyline points="18 15 12 9 6 15"/>
      </svg>
    </button>
  `
})
export class App implements OnInit, OnDestroy {
  private mouseX = 0;
  private mouseY = 0;
  private glowX = 0;
  private glowY = 0;
  private animationFrameId: number | null = null;
  private mouseListener: any;
  private scrollListener: any;
  showBackToTop = false;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.mouseListener = (e: MouseEvent) => {
        this.mouseX = e.clientX;
        this.mouseY = e.clientY;
      };
      document.addEventListener('mousemove', this.mouseListener);
      
      this.scrollListener = () => {
        this.showBackToTop = window.scrollY > 500;
      };
      window.addEventListener('scroll', this.scrollListener, { passive: true });
      
      this.animateGlow();
    }
  }

  animateGlow = () => {
    const cursorGlow = document.getElementById('cursor-glow');
    if (cursorGlow) {
      this.glowX += (this.mouseX - this.glowX) * 0.08;
      this.glowY += (this.mouseY - this.glowY) * 0.08;
      cursorGlow.style.left = this.glowX + 'px';
      cursorGlow.style.top = this.glowY + 'px';
    }
    this.animationFrameId = requestAnimationFrame(this.animateGlow);
  };

  scrollToTop() {
    if (isPlatformBrowser(this.platformId)) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  ngOnDestroy() {
    if (isPlatformBrowser(this.platformId)) {
      document.removeEventListener('mousemove', this.mouseListener);
      window.removeEventListener('scroll', this.scrollListener);
      if (this.animationFrameId !== null) {
        cancelAnimationFrame(this.animationFrameId);
      }
    }
  }
}

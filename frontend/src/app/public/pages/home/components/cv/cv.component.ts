import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-home-cv',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cv.component.html',
  styleUrls: ['./cv.component.scss']
})
export class CvComponent implements OnInit {
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
        document.querySelectorAll('#cv-section .reveal').forEach(el => revealObs.observe(el));
      }, 100);
    }
  }

  downloadCV(): void {
    const link = document.createElement('a');
    link.href = 'uploads/cv/Abdullah_Mohammed_CV.pdf';
    link.download = 'Abdullah_Mohammed_CV.pdf';
    link.click();
    this.showToast();
  }

  previewCV(): void {
    window.open('uploads/cv/Abdullah_Mohammed_CV.pdf', '_blank');
  }

  private showToast(): void {
    const toast = document.getElementById('cv-toast');
    if (toast) {
      toast.classList.add('show');
      setTimeout(() => toast.classList.remove('show'), 3000);
    }
  }
}
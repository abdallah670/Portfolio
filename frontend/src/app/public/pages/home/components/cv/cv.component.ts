import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ApiService } from '../../../../../core/services/api.service';

@Component({
  selector: 'app-home-cv',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cv.component.html',
  styleUrls: ['./cv.component.scss']
})
export class CvComponent implements OnInit {
  private isBrowser: boolean;
  private readonly API_URL = 'https://menoportfolio.runasp.net';

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private api: ApiService
  ) {
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
    // First check if CV exists by fetching it
    fetch(`${this.API_URL}/api/portfolio/cv`)
      .then(response => {
        if (!response.ok) {
          throw new Error('CV not found');
        }
        return response.blob();
      })
      .then(blob => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'Abdullah_Mohammed_CV.pdf';
        link.click();
        window.URL.revokeObjectURL(url);
        this.showToast();
      })
      .catch(err => {
        console.error('Failed to download CV:', err);
        this.showErrorToast('CV not available. Please upload it in the admin settings.');
      });
  }

  previewCV(): void {
    const cvUrl = `${this.API_URL}/api/portfolio/cv/preview`;
    window.open(cvUrl);
  }

  private showToast(): void {
    const toast = document.getElementById('cv-toast');
    if (toast) {
      toast.classList.add('show');
      setTimeout(() => toast.classList.remove('show'), 3000);
    }
  }

  private showErrorToast(message: string): void {
    const toast = document.getElementById('cv-toast');
    if (toast) {
      toast.textContent = message;
      toast.classList.add('show', 'error');
      setTimeout(() => {
        toast.classList.remove('show', 'error');
        toast.textContent = 'CV downloaded successfully!';
      }, 3000);
    }
  }
}
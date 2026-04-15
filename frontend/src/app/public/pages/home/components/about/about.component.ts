import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';

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

 

  get displaySubtitle(): string {
    return "A backend developer focused on building reliable and scalable systems.";
  }

  
}

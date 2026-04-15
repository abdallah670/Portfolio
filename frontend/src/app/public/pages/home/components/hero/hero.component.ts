import { Component, Input, OnInit, OnDestroy, Inject, PLATFORM_ID, Pipe, PipeTransform, ElementRef, ViewChild, AfterViewInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Subject } from 'rxjs';
import { HeroConfig } from '../../../../../core/models/portfolio.models';

@Pipe({
  name: 'toChars',
  standalone: true
})
export class ToCharsPipe implements PipeTransform {
  transform(value: string): string[] {
    return value ? value.split('').map(c => c === ' ' ? '\u00A0' : c) : [];
  }
}

@Component({
  selector: 'app-home-hero',
  standalone: true,
  imports: [CommonModule, ToCharsPipe],
  templateUrl: './hero.component.html',
  styleUrls: ['./hero.component.scss']
})
export class HeroComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() hero?: HeroConfig;
  @ViewChild('particlesCanvas') particlesCanvas!: ElementRef<HTMLCanvasElement>;

  typedText = '';
  typewriterActive = false;
  private lines = [
    'Backend-focused .NET developer.',
    'Building scalable systems in C# & SQL.',
    'Clean architecture — from data to UI.',
    'From Cairo, Egypt — open worldwide.',
  ];
  private lineIndex = 0;
  private charIndex = 0;
  private isDeleting = false;
  private typeInterval: any;
  private isBrowser: boolean;
  private particleAnimation: any;
  private destroy$ = new Subject<void>();

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object,
    private cdr: ChangeDetectorRef
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser) {
      this.typewriterActive = true;
      setTimeout(() => {
        this.typeLoop();
      }, 1000);
    }
  }

  ngAfterViewInit(): void {
    if (this.isBrowser) {
      setTimeout(() => {
        this.initParticles();
        this.initCounters();
      }, 100);
    }
  }

  private typeLoop(): void {
    if (!this.isBrowser || !this.typewriterActive) return;
    
    const line = this.lines[this.lineIndex];
    
    if (this.isDeleting) {
      this.typedText = line.substring(0, --this.charIndex);
    } else {
      this.typedText = line.substring(0, ++this.charIndex);
    }
    
    // Force Angular to check the view
    this.cdr.detectChanges();

    if (!this.isDeleting && this.charIndex === line.length) {
      this.isDeleting = true;
      this.typeInterval = setTimeout(() => this.typeLoop(), 2400);
      return;
    }

    if (this.isDeleting && this.charIndex === 0) {
      this.isDeleting = false;
      this.lineIndex = (this.lineIndex + 1) % this.lines.length;
    }

    const delay = this.isDeleting ? 38 : 62;
    this.typeInterval = setTimeout(() => this.typeLoop(), delay);
  }

  private initCounters(): void {
    const animateCounter = (el: HTMLElement, target: number, suffix: string, duration = 1200) => {
      let startTime: number | null = null;
      const step = (timestamp: number) => {
        if (!startTime) startTime = timestamp;
        const progress = Math.min((timestamp - startTime) / duration, 1);
        const eased = 1 - Math.pow(1 - progress, 3);
        const value = Math.floor(eased * target);
        el.innerHTML = value + '<span>' + suffix + '</span>';
        if (progress < 1) requestAnimationFrame(step);
      };
      requestAnimationFrame(step);
    };

    if ('IntersectionObserver' in window) {
      const counterObs = new IntersectionObserver((entries) => {
        entries.forEach(e => {
          if (e.isIntersecting) {
            const el = e.target as HTMLElement;
            const target = +(el.getAttribute('data-count') || 0);
            const suffix = el.getAttribute('data-suffix') || '';
            animateCounter(el, target, suffix);
            counterObs.unobserve(el);
          }
        });
      }, { threshold: 0.5 });
      
      document.querySelectorAll('[data-count]').forEach(el => counterObs.observe(el));
    }
  }

  ngOnDestroy(): void {
    this.typewriterActive = false;
    if (this.typeInterval) {
      clearTimeout(this.typeInterval);
    }
    if (this.particleAnimation) {
      cancelAnimationFrame(this.particleAnimation);
    }
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initParticles(): void {
    const canvas = this.particlesCanvas?.nativeElement;
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    let W = canvas.width = canvas.offsetWidth;
    let H = canvas.height = canvas.offsetHeight;
    let particles: any[] = [];

    const resize = () => {
      W = canvas.width = canvas.offsetWidth;
      H = canvas.height = canvas.offsetHeight;
    };
    window.addEventListener('resize', resize);

    class Particle {
      x = Math.random() * W;
      y = Math.random() * H;
      size = Math.random() * 1.5 + 0.3;
      speedX = (Math.random() - 0.5) * 0.3;
      speedY = (Math.random() - 0.5) * 0.3;
      opacity = Math.random() * 0.4 + 0.05;
      life = Math.random() * 200 + 100;
      age = 0;
      
      reset() {
        this.x = Math.random() * W;
        this.y = Math.random() * H;
        this.size = Math.random() * 1.5 + 0.3;
        this.speedX = (Math.random() - 0.5) * 0.3;
        this.speedY = (Math.random() - 0.5) * 0.3;
        this.opacity = Math.random() * 0.4 + 0.05;
        this.life = Math.random() * 200 + 100;
        this.age = 0;
      }
      
      update() {
        this.x += this.speedX;
        this.y += this.speedY;
        this.age++;
        if (this.age > this.life || this.x < 0 || this.x > W || this.y < 0 || this.y > H) {
          this.reset();
        }
      }
      
      draw() {
        if (!ctx) return;
        ctx.save();
        ctx.globalAlpha = this.opacity * (1 - this.age / this.life);
        ctx.fillStyle = '#3b82f6';
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
        ctx.fill();
        ctx.restore();
      }
    }

    for (let i = 0; i < 60; i++) {
      particles.push(new Particle());
    }

    const connect = () => {
      for (let i = 0; i < particles.length; i++) {
        for (let j = i + 1; j < particles.length; j++) {
          const dx = particles[i].x - particles[j].x;
          const dy = particles[i].y - particles[j].y;
          const dist = Math.sqrt(dx * dx + dy * dy);
          if (dist < 100) {
            ctx.save();
            ctx.globalAlpha = (1 - dist / 100) * 0.08;
            ctx.strokeStyle = '#3b82f6';
            ctx.lineWidth = 0.5;
            ctx.beginPath();
            ctx.moveTo(particles[i].x, particles[i].y);
            ctx.lineTo(particles[j].x, particles[j].y);
            ctx.stroke();
            ctx.restore();
          }
        }
      }
    };

    const animate = () => {
      ctx.clearRect(0, 0, W, H);
      particles.forEach(p => { p.update(); p.draw(); });
      connect();
      this.particleAnimation = requestAnimationFrame(animate);
    };
    animate();
  }

  get parsedStats() {
    if (!this.hero?.stats?.length) {
      return [
        { label: 'Projects', count: 5, suffix: '+', isNumeric: true },
        { label: 'Backend Focus', count: 100, suffix: '%', isNumeric: true },
        { label: 'SQL Expertise', count: 0, suffix: 'Advanced', isNumeric: false },
      ];
    }
    return this.hero.stats.map(s => {
      const match = s.value.trim().match(/^(\d+)(.*)$/);
      if (match) {
        return { label: s.label, count: parseInt(match[1], 10), suffix: match[2].trim(), isNumeric: true };
      }
      return { label: s.label, count: 0, suffix: s.value, isNumeric: false };
    });
  }

  scrollTo(elementId: string, event?: Event): void {
    if (event) {
      event.preventDefault();
    }
    if (this.isBrowser) {
      const element = document.getElementById(elementId);
      if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
      }
    }
  }

  get imageUrl(): string {
    if (!this.hero?.profileImage) return '';
    if (this.hero.profileImage.startsWith('http')) return this.hero.profileImage;
    return `http://localhost:5000/${this.hero.profileImage}`;
  }

  onMouseMove(event: MouseEvent): void {
    if (!this.isBrowser) return;
    const card = document.querySelector('.profile-card') as HTMLElement;
    if (!card) return;
    
    const rect = card.getBoundingClientRect();
    const x = (event.clientX - rect.left) / rect.width - 0.5;
    const y = (event.clientY - rect.top) / rect.height - 0.5;
    
    card.style.transform = `rotateY(${x * 12}deg) rotateX(${-y * 8}deg) scale(1.02)`;
    card.style.boxShadow = `${-x * 20}px ${y * 20}px 60px rgba(59,130,246,0.2)`;
  }

  onMouseLeave(): void {
    if (!this.isBrowser) return;
    const card = document.querySelector('.profile-card') as HTMLElement;
    if (!card) return;
    
    card.style.transform = 'rotateY(0deg) rotateX(0deg) scale(1)';
    card.style.boxShadow = '';
    card.style.transition = 'all 0.5s ease';
    setTimeout(() => card.style.transition = '', 500);
  }
}
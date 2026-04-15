import { Component, Input, OnInit, OnDestroy, Inject, PLATFORM_ID, Pipe, PipeTransform, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { HeroConfig } from '../../../../../core/models/portfolio.models';

@Pipe({
  name: 'toChars',
  standalone: true
})
export class ToCharsPipe implements PipeTransform {
  transform(value: string): string[] {
    return value ? value.split('') : [];
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

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    if (this.isBrowser && !this.hero) {
      setTimeout(() => this.type(), 1000);
    }
  }

  ngAfterViewInit(): void {
    if (this.isBrowser) {
      setTimeout(() => this.initParticles(), 100);
    }
  }

  ngOnDestroy(): void {
    if (this.typeInterval) {
      clearTimeout(this.typeInterval);
    }
    if (this.particleAnimation) {
      cancelAnimationFrame(this.particleAnimation);
    }
  }

  private type(): void {
    const line = this.lines[this.lineIndex];
    
    if (this.isDeleting) {
      this.typedText = line.substring(0, --this.charIndex);
    } else {
      this.typedText = line.substring(0, ++this.charIndex);
    }

    if (!this.isDeleting && this.charIndex === line.length) {
      this.isDeleting = true;
      this.typeInterval = setTimeout(() => this.type(), 2400);
      return;
    }

    if (this.isDeleting && this.charIndex === 0) {
      this.isDeleting = false;
      this.lineIndex = (this.lineIndex + 1) % this.lines.length;
    }

    const delay = this.isDeleting ? 38 : 62;
    this.typeInterval = setTimeout(() => this.type(), delay);
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
    }

    const animate = () => {
      ctx.clearRect(0, 0, W, H);
      particles.forEach(p => { p.update(); p.draw(); });
      connect();
      this.particleAnimation = requestAnimationFrame(animate);
    };
    animate();
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
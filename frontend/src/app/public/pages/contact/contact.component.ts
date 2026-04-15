import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { NavbarComponent } from '../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../shared/components/footer/footer.component';
import { ApiService } from '../../../core/services/api.service';
import { ContactConfig, SocialLinkConfig, CreateMessageRequest } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent implements OnInit {
  contact?: ContactConfig;
  socials: SocialLinkConfig[] = [];

  // Form data
  form: CreateMessageRequest = {
    name: '',
    email: '',
    subject: '',
    content: ''
  };

  loading = false;
  success = false;
  error = '';

  private isBrowser: boolean;

  constructor(
    private api: ApiService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  ngOnInit(): void {
    this.loadContactData();
    if (this.isBrowser) {
      this.initScrollReveal();
    }
  }

  private loadContactData(): void {
    this.api.getPortfolioConfig().subscribe({
      next: (config) => {
        this.contact = config.contact;
        this.socials = config.socials || [];
      },
      error: (err) => {
        console.error('Failed to load contact data:', err);
      }
    });
  }

  onSubmit(event: Event): void {
    event.preventDefault();

    // Validation
    if (!this.form.name || !this.form.email || !this.form.content) {
      this.error = 'Please fill in all required fields (Name, Email, Message).';
      return;
    }

    if (!this.isValidEmail(this.form.email)) {
      this.error = 'Please enter a valid email address.';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = false;

    this.api.sendMessage(this.form).subscribe({
      next: () => {
        this.success = true;
        this.loading = false;
        this.form = { name: '', email: '', subject: '', content: '' };
      },
      error: (err) => {
        this.error = 'Failed to send message. Please try again later.';
        this.loading = false;
        console.error('Error sending message:', err);
      }
    });
  }

  private isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }

  get whatsappLink(): string {
    const phone = this.contact?.whatsApp?.replace(/\D/g, '');
    return phone ? `https://wa.me/${phone}` : '#';
  }

  getSocialIcon(iconName: string): string {
    const iconMap: Record<string, string> = {
      'github': 'code',
      'linkedin': 'link',
      'instagram': 'share',
      'twitter': 'flutter_dash',
      'facebook': 'thumb_up',
      'youtube': 'play_arrow',
      'email': 'mail',
      'website': 'language'
    };
    return iconMap[iconName?.toLowerCase()] || 'link';
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
        document.querySelectorAll('.reveal').forEach(el => revealObs.observe(el));
      }, 100);
    }
  }
}

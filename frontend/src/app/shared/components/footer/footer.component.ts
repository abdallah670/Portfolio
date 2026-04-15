import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactConfig, SocialLinkConfig } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer>
      <p class="footer-copy">© 2025 <span>Abdullah Mohammed</span> — Built with clean code &amp; attention to detail.</p>
      <nav class="footer-links">
        <a href="https://github.com/abdallah670" target="_blank">GitHub</a>
        <a href="https://linkedin.com/in/abdullah-mohammed-334475294" target="_blank">LinkedIn</a>
        <a href="mailto:meno.mo.dev@gmail.com">Email</a>
      </nav>
    </footer>
  `,
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {
  @Input() contact?: ContactConfig;
  @Input() socials: SocialLinkConfig[] = [];

  get email(): string | null {
    return this.contact?.email ? `mailto:${this.contact.email}` : null;
  }
}
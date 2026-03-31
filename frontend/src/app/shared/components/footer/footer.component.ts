import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="footer-section">
      <div class="container footer-content">
        <div class="footer-brand">
          <div class="footer-logo">Abdullah.dev</div>
          <div class="footer-copyright">
            © 2024 Abdullah Mohammed. All rights reserved.
          </div>
        </div>
        <div class="footer-socials">
          <a href="#" class="social-link">Github</a>
          <a href="#" class="social-link">LinkedIn</a>
          <a href="#" class="social-link">Instagram</a>
        </div>
      </div>
    </footer>
  `,
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent {}

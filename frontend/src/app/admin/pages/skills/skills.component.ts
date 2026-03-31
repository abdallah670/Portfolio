import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page">
      <h1>Skills & Tools</h1>
      <p>Manage your skills and tech stack. (Full implementation coming soon)</p>
    </div>
  `,
  styles: [`
    .page {
      padding: 24px;
    }
    h1 {
      font-size: 24px;
      font-weight: 700;
      margin-bottom: 16px;
      color: var(--foreground);
    }
    p {
      color: var(--muted-foreground);
    }
  `]
})
export class SkillsComponent {}
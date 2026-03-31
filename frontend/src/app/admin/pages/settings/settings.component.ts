import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent {
  skills = ['Angular', 'TailwindCSS', 'TypeScript', 'Node.js'];

  removeSkill(skillToRemove: string) {
    this.skills = this.skills.filter(s => s !== skillToRemove);
  }
}
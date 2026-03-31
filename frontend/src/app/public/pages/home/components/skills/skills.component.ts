import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home-skills',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './skills.component.html',
  styleUrls: ['./skills.component.scss']
})
export class SkillsComponent {
  skillCategories = [
    {
      name: 'Backend Development',
      accentClass: 'emerald',
      skills: [
        { label: 'C#', level: 85 },
        { label: '.NET', level: 80 },
        { label: 'ADO.NET', level: 80 },
        { label: 'REST APIs', level: 75 }
      ]
    },
    {
      name: 'Database',
      accentClass: 'cyan',
      skills: [
        { label: 'SQL Server', level: 85 },
        { label: 'Query Optimization', level: 75 },
        { label: 'Database Design', level: 80 },
        { label: 'Stored Procedures', level: 75 }
      ]
    },
    {
      name: 'Software Engineering',
      accentClass: 'purple',
      skills: [
        { label: 'Design Patterns', level: 70 },
        { label: 'Layered Architecture', level: 80 },
        { label: 'OOP', level: 85 },
        { label: 'Debugging', level: 85 }
      ]
    },
    {
      name: 'Frontend Development',
      accentClass: 'blue',
      skills: [
        { label: 'Angular', level: 70 },
        { label: 'TypeScript', level: 75 },
        { label: 'HTML/CSS', level: 75 },
        { label: 'RxJS', level: 60 }
      ]
    }
  ];
}

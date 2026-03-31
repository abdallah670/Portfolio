import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home-about',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent {
  aboutCards = [
    { title: '.NET Developer', subtitle: 'C#, ADO.NET, Windows Services' },
    { title: 'Database Specialist', subtitle: 'SQL Server, Query Optimization' },
    { title: 'System Design', subtitle: 'Layered Architecture, Clean Code' },
    { title: 'Problem Solver', subtitle: 'Real-world system implementation' }
  ];

  achievements = [
    'Built online coaching system with transaction handling',
    'Implemented layered architecture across DAL, Business, and DTO layers',
    'Designed SQL databases and Entity Framework models for real application flows',
    'Hands-on with real system logic, business rules, and practical constraints'
  ];

  values = [
    { title: 'Clean Architecture', desc: 'Focus on separation of concerns and maintainable system design.' },
    { title: 'Data Integrity', desc: 'Strong emphasis on correct data handling and database design.' },
    { title: 'Scalability', desc: 'Building systems that can grow without breaking.' }
  ];
}

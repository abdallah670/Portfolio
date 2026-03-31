import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home-projects',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent {
  projects = [
    {
      title: 'MenoPro Gym System',
      description: 'Premium gym management with member/trainer portals, workout plans, diet tracking, and Chart.js analytics. Features Glassmorphism UI, Stripe payments, and AI integration.',
      tags: ['ASP.NET Core', 'Chart.js'],
      image: 'https://storage.googleapis.com/banani-generated-images/generated-images/9bcd0d6e-ee0c-4116-beda-d5db2e6d31de.jpg',
      isWide: true,
      link: '#'
    },
    {
      title: 'Labor Marketplace',
      description: 'Platform connecting workers with job posters. Features multi-role auth, real-time chat with SignalR, Stripe payments, and spatial queries.',
      tags: ['.NET 9', 'SignalR'],
      image: 'https://storage.googleapis.com/banani-generated-images/generated-images/5e723a8f-9fdc-4413-85d9-fe1b83024562.jpg',
      isWide: false,
      link: '#'
    },
    {
      title: 'Outfit Planner',
      description: 'Intelligent wardrobe management system that generates outfit suggestions by analyzing clothes against real-time weather, occasions, and style preferences.',
      tags: ['Angular 17+', 'CQRS'],
      image: 'https://storage.googleapis.com/banani-generated-images/generated-images/f0ac8fce-53ae-4b58-912f-d7cb3a7b87aa.jpg',
      isWide: false,
      link: '#'
    }
  ];
}

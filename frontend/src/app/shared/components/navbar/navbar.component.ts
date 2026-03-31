import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="navbar-wrapper">
      <div class="container">
        <nav class="navbar">
          <a routerLink="/" class="navbar-logo">
            Abdullah.dev
          </a>
          <div class="navbar-links">
            <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}" class="nav-link">Home</a>
            <a routerLink="/projects" routerLinkActive="active" class="nav-link">Projects</a>
            <a routerLink="/about" routerLinkActive="active" class="nav-link">About</a>
            <a routerLink="/contact" routerLinkActive="active" class="nav-link">Contact</a>
          </div>
          <button class="btn btn-primary btn-sm" routerLink="/admin">
            Admin
          </button>
        </nav>
      </div>
    </header>
  `,
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent {}

import { Routes } from '@angular/router';

export const routes: Routes = [
  // Public routes (portfolio)
  {
    path: '',
    loadComponent: () => import('./public/pages/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'contact',
    loadComponent: () => import('./public/pages/contact/contact.component').then(m => m.ContactComponent)
  },
  
  // Admin routes
  {
    path: 'admin',
    loadChildren: () => import('./admin/admin.routes').then(m => m.adminRoutes)
  },
  
  // Login route
  {
    path: 'login',
    loadComponent: () => import('./public/pages/login/login.component').then(m => m.LoginComponent)
  },
  
  // Redirect unknown routes to home
  { path: '**', redirectTo: '' }
];
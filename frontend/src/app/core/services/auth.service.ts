import { Injectable, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'portfolio-token';
  private isAuthenticatedSignal = signal<boolean>(this.hasToken());
  
  readonly isAuthenticated = computed(() => this.isAuthenticatedSignal());
  
  constructor(private apiService: ApiService, private router: Router) {}
  
  login(username: string, password: string): Promise<boolean> {
    return new Promise((resolve) => {
      this.apiService.login(username, password).subscribe({
        next: (response) => {
          localStorage.setItem(this.TOKEN_KEY, response.token);
          this.isAuthenticatedSignal.set(true);
          resolve(true);
        },
        error: () => {
          resolve(false);
        }
      });
    });
  }
  
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.isAuthenticatedSignal.set(false);
    this.router.navigate(['/admin/login']);
  }
  
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }
  
  private hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }
}
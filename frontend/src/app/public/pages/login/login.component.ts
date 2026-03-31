import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="login-page">
      <div class="login-card">
        <div class="login-header">
          <h1>Welcome Back</h1>
          <p>Sign in to access your admin dashboard</p>
        </div>
        
        <form (ngSubmit)="onSubmit()" class="login-form">
          <div class="form-group">
            <label for="username">Username</label>
            <input 
              type="text" 
              id="username"
              [(ngModel)]="username"
              name="username"
              placeholder="Enter your username"
              required
            />
          </div>
          
          <div class="form-group">
            <label for="password">Password</label>
            <input 
              type="password" 
              id="password"
              [(ngModel)]="password"
              name="password"
              placeholder="Enter your password"
              required
            />
          </div>
          
          <div *ngIf="error" class="error-message">
            {{ error }}
          </div>
          
          <button type="submit" class="btn-login" [disabled]="loading">
            {{ loading ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>
        
        <div class="login-footer">
          <a routerLink="/">← Back to Portfolio</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-page {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: var(--background);
      padding: 24px;
    }

    .login-card {
      width: 100%;
      max-width: 400px;
      background: var(--card);
      border: 1px solid var(--border);
      border-radius: var(--radius-lg);
      padding: 40px;
    }

    .login-header {
      text-align: center;
      margin-bottom: 32px;
    }

    .login-header h1 {
      font-size: 24px;
      font-weight: 700;
      color: var(--foreground);
      margin-bottom: 8px;
    }

    .login-header p {
      color: var(--muted-foreground);
    }

    .login-form {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }

    .form-group label {
      font-size: 14px;
      font-weight: 500;
      color: var(--foreground);
    }

    .form-group input {
      padding: 10px 14px;
      border: 1px solid var(--border);
      border-radius: var(--radius-md);
      background: var(--input);
      color: var(--foreground);
      font-size: 14px;
      font-family: inherit;
    }

    .form-group input:focus {
      outline: none;
      border-color: var(--primary);
    }

    .form-group input::placeholder {
      color: var(--muted-foreground);
    }

    .error-message {
      padding: 12px;
      background: var(--destructive);
      color: var(--destructive-foreground);
      border-radius: var(--radius-md);
      font-size: 14px;
    }

    .btn-login {
      padding: 12px;
      background: var(--primary);
      color: var(--primary-foreground);
      border: none;
      border-radius: var(--radius-md);
      font-size: 14px;
      font-weight: 600;
      cursor: pointer;
      transition: opacity 0.2s;
    }

    .btn-login:hover:not(:disabled) {
      opacity: 0.9;
    }

    .btn-login:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .login-footer {
      margin-top: 24px;
      text-align: center;
    }

    .login-footer a {
      color: var(--muted-foreground);
      text-decoration: none;
      font-size: 14px;
      transition: color 0.2s;
    }

    .login-footer a:hover {
      color: var(--foreground);
    }
  `]
})
export class LoginComponent {
  username = '';
  password = '';
  error = '';
  loading = false;

  constructor(private authService: AuthService, private router: Router) {}

  async onSubmit(): Promise<void> {
    if (!this.username || !this.password) {
      this.error = 'Please enter both username and password';
      return;
    }

    this.loading = true;
    this.error = '';

    try {
      const success = await this.authService.login(this.username, this.password);
      if (success) {
        this.router.navigate(['/admin/dashboard']);
      } else {
        this.error = 'Invalid username or password';
      }
    } catch {
      this.error = 'An error occurred. Please try again.';
    } finally {
      this.loading = false;
    }
  }
}
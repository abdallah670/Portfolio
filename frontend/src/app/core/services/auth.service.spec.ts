import { fakeAsync, tick } from '@angular/core/testing';
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { ApiService } from './api.service';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './auth.interceptor';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        ApiService,
        provideHttpClient(withInterceptors([authInterceptor])),
      ],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    localStorage.clear();
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('isAuthenticated', () => {
    it('should return false when no token exists', () => {
      localStorage.clear();
      expect(service.isAuthenticated()).toBe(false);
    });

    it('should return true when token exists', () => {
      localStorage.setItem('portfolio-token', 'test-token');
      expect(service.isAuthenticated()).toBe(true);
    });
  });

  describe('getToken', () => {
    it('should return null when no token stored', () => {
      localStorage.clear();
      expect(service.getToken()).toBeNull();
    });

    it('should return token when stored', () => {
      const token = 'test-token-123';
      localStorage.setItem('portfolio-token', token);
      expect(service.getToken()).toBe(token);
    });
  });

  describe('login', () => {
    it('should login successfully and store token', fakeAsync(() => {
      const mockResponse = { token: 'jwt-token-123' };
      
      service.login('testuser', 'password123');
      
      const req = httpMock.expectOne('/api/auth/login');
      req.flush(mockResponse);
      
      tick();
      
      expect(service.getToken()).toBe('jwt-token-123');
    }));
  });

  describe('logout', () => {
    it('should clear token on logout', fakeAsync(() => {
      localStorage.setItem('portfolio-token', 'test-token');
      
      service.logout();
      
      tick();
      
      expect(service.getToken()).toBeNull();
    }));
  });
});
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiService } from './api.service';
import { PortfolioConfig, Message, CreateMessageRequest } from '../models/portfolio.models';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiService],
    });

    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getPortfolioConfig', () => {
    it('should return portfolio configuration', () => {
      const mockConfig: PortfolioConfig = {
        hero: {
          name: 'John',
          headlineTop: 'Welcome',
          headlineMain: 'My Portfolio',
          availabilityLabel: 'Available',
          subtitle: 'Developer',
          profileImage: '/img.jpg',
          stats: [],
        },
        skills: [],
        featuredProjects: [],
        moreProjects: [],
        journey: [],
        socials: [],
        contact: {
          email: 'test@test.com',
          whatsApp: '123456',
          phone: '123',
          location: 'Test',
        },
      };

      service.getPortfolioConfig().subscribe((config) => {
        expect(config).toEqual(mockConfig);
      });

      const req = httpMock.expectOne('/api/portfolio/config');
      req.flush(mockConfig);
    });
  });

  describe('sendMessage', () => {
    it('should send contact message', () => {
      const message: CreateMessageRequest = {
        name: 'John Doe',
        email: 'john@test.com',
        subject: 'Test',
        content: 'Test message',
      };

      const mockResponse = { success: true, message: 'Message sent' };

      service.sendMessage(message).subscribe((response) => {
        expect(response.success).toBeTrue();
      });

      const req = httpMock.expectOne('/api/messages');
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
    });
  });

  describe('login', () => {
    it('should authenticate user and return token', () => {
      const mockResponse = { success: true, token: 'jwt-token' };

      service.login('admin', 'password').subscribe((response) => {
        expect(response.token).toBe('jwt-token');
      });

      const req = httpMock.expectOne('/api/auth/login');
      req.flush(mockResponse);
    });
  });
});
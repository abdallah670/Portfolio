import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Project, 
  Message, 
  SkillCategory, 
  Hero, 
  About, 
  JourneyItem, 
  Contact, 
  SocialLink,
  DashboardStats,
  PaginatedResponse,
  CreateMessageRequest
} from '../models/portfolio.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly API_URL = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  // Auth
  login(username: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.API_URL}/auth/login`, { username, password });
  }

  // Portfolio Config (Public)
  getPortfolioConfig(): Observable<any> {
    return this.http.get(`${this.API_URL}/portfolio/config`);
  }

  getSkills(): Observable<SkillCategory[]> {
    return this.http.get<SkillCategory[]>(`${this.API_URL}/portfolio/skills`);
  }

  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.API_URL}/portfolio/projects`);
  }

  // Dashboard Stats (Admin)
  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.API_URL}/portfolio/dashboard-stats`);
  }

  // Projects (Admin)
  createProject(project: Partial<Project>): Observable<Project> {
    return this.http.post<Project>(`${this.API_URL}/portfolio/projects`, project);
  }

  updateProject(project: Project): Observable<Project> {
    return this.http.put<Project>(`${this.API_URL}/portfolio/projects`, project);
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/portfolio/projects/${id}`);
  }

  // Hero (Admin)
  updateHero(hero: Hero): Observable<Hero> {
    return this.http.put<Hero>(`${this.API_URL}/portfolio/hero`, hero);
  }

  // About (Admin)
  updateAbout(about: About): Observable<About> {
    return this.http.put<About>(`${this.API_URL}/portfolio/about`, about);
  }

  // Journey (Admin)
  createJourney(item: Partial<JourneyItem>): Observable<JourneyItem> {
    return this.http.post<JourneyItem>(`${this.API_URL}/portfolio/journey`, item);
  }

  updateJourney(item: JourneyItem): Observable<JourneyItem> {
    return this.http.put<JourneyItem>(`${this.API_URL}/portfolio/journey`, item);
  }

  deleteJourney(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/portfolio/journey/${id}`);
  }

  // Contact (Admin)
  updateContact(contact: Contact): Observable<Contact> {
    return this.http.put<Contact>(`${this.API_URL}/portfolio/contact`, contact);
  }

  // Social Links (Admin)
  createSocial(social: Partial<SocialLink>): Observable<SocialLink> {
    return this.http.post<SocialLink>(`${this.API_URL}/portfolio/socials`, social);
  }

  updateSocial(social: SocialLink): Observable<SocialLink> {
    return this.http.put<SocialLink>(`${this.API_URL}/portfolio/socials`, social);
  }

  deleteSocial(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/portfolio/socials/${id}`);
  }

  // Skills (Admin)
  createSkillCategory(category: Partial<SkillCategory>): Observable<SkillCategory> {
    return this.http.post<SkillCategory>(`${this.API_URL}/portfolio/skills/categories`, category);
  }

  updateSkillCategory(category: SkillCategory): Observable<SkillCategory> {
    return this.http.put<SkillCategory>(`${this.API_URL}/portfolio/skills/categories`, category);
  }

  createSkill(skill: { name: string; level: number; categoryId: number }): Observable<any> {
    return this.http.post(`${this.API_URL}/portfolio/skills`, skill);
  }

  deleteSkill(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/portfolio/skills/${id}`);
  }

  // Messages (Public - Contact Form)
  sendMessage(message: CreateMessageRequest): Observable<{ message: string; id: number }> {
    return this.http.post<{ message: string; id: number }>(`${this.API_URL}/messages`, message);
  }

  // Messages (Admin)
  getMessages(page: number = 1, pageSize: number = 20, isRead?: boolean): Observable<PaginatedResponse<Message>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (isRead !== undefined) {
      params = params.set('isRead', isRead.toString());
    }
    
    return this.http.get<PaginatedResponse<Message>>(`${this.API_URL}/messages`, { params });
  }

  getMessage(id: number): Observable<Message> {
    return this.http.get<Message>(`${this.API_URL}/messages/${id}`);
  }

  markMessageAsRead(id: number): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/messages/${id}/read`, {});
  }

  deleteMessage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/messages/${id}`);
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<number>(`${this.API_URL}/messages/unread-count`);
  }

  // File Upload
  uploadProjectImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(`${this.API_URL}/upload/project-image`, formData);
  }

  uploadProfileImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(`${this.API_URL}/upload/profile-image`, formData);
  }
}
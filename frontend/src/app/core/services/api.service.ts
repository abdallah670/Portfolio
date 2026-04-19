import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Project, 
  Message, 
  SkillCategory, 
  Hero, 
  HeroStatConfig,
  JourneyItem, 
  Contact, 
  SocialLink,
  DashboardStats,
  PaginatedResponse,
  CreateMessageRequest,
  SystemSetting,
  PortfolioConfig
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
  getPortfolioConfig(): Observable<PortfolioConfig> {
    return this.http.get<PortfolioConfig>(`${this.API_URL}/portfolio/config`);
  }

   getSkills(): Observable<SkillCategory[]> {
     return this.http.get<SkillCategory[]>(`${this.API_URL}/portfolio/skills`);
   }
 
   // Dashboard Stats (Admin)
   getDashboardStats(): Observable<DashboardStats> {
     return this.http.get<DashboardStats>(`${this.API_URL}/portfolio/dashboard-stats`);
   }
 
   getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.API_URL}/portfolio/projects`);
  }

  // Projects (Admin - All including drafts)
  getAllProjectsAdmin(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<Project>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PaginatedResponse<Project>>(`${this.API_URL}/portfolio/admin/projects`, { params });
  }

 
  // Projects (Admin)
  createProject(project: Partial<Project>): Observable<Project> {
    return this.http.post<Project>(`${this.API_URL}/portfolio/projects`, project);
  }

  updateProject(project: Project): Observable<Project> {
    return this.http.put<Project>(`${this.API_URL}/portfolio/projects/${project.id}`, project);
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/portfolio/projects/${id}`);
  }
  
  // Hero (Admin)
   // Dashboard Stats (Admin)
  getProfileImage(): Observable<string> {
    return this.http.get(`${this.API_URL}/portfolio/ProfileImage`, { responseType: 'text' });
  }

  // CV
  getCV(): Observable<Blob> {
    return this.http.get(`${this.API_URL}/portfolio/cv`, { responseType: 'blob' });
  }

  uploadCV(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(`${this.API_URL}/upload/cv`, formData);
  }

  updateHero(hero: Hero, stats?: HeroStatConfig[]): Observable<Hero> {
    return this.http.put<Hero>(`${this.API_URL}/portfolio/hero`, { hero, stats });
  }

   // Journey (Admin)
   getJourney(): Observable<JourneyItem[]> {
     return this.http.get<JourneyItem[]>(`${this.API_URL}/portfolio/journey`);
   }

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

  // Project Views (Public)
  incrementProjectViews(projectId: number): Observable<void> {
    return this.http.post<void>(`${this.API_URL}/portfolio/projects/${projectId}/views`, {});
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

  deleteSkillCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/portfolio/skills/categories/${id}`);
  }

  createSkill(skill: { name: string; level: number; categoryId: number }): Observable<any> {
    return this.http.post(`${this.API_URL}/portfolio/skills`, skill);
  }

  deleteSkill(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/portfolio/skills/${id}`);
  }

  updateSkill(skill: { id?: number; name: string; level: number; categoryId: number }): Observable<any> {
    return this.http.put(`${this.API_URL}/portfolio/skills`, skill);
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

   markAllMessagesAsRead(): Observable<{ markedAsReadCount: number }> {
     return this.http.put<{ markedAsReadCount: number }>(`${this.API_URL}/messages/read-all`, {});
   }

   deleteMessage(id: number): Observable<void> {
     return this.http.delete<void>(`${this.API_URL}/messages/${id}`);
   }

   // Reply to message
   respondToMessage(id: number, content: string): Observable<{ message: string }> {
     return this.http.post<{ message: string }>(`${this.API_URL}/messages/${id}/respond`, { content });
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

  uploadProfileImage(file: File): Observable<{ data: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ data: string }>(`${this.API_URL}/upload/profile-image`, formData);
  }

  // Settings
  getSettings(category?: string): Observable<SystemSetting[]> {
    let params = new HttpParams();
    if (category) params = params.set('category', category);
    return this.http.get<SystemSetting[]>(`${this.API_URL}/settings`, { params });
  }

  updateSetting(key: string, value: string, dataType: string = 'string'): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/settings`, { key, value, dataType });
  }

  // Username Change
  updateUsername(newUsername: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.API_URL}/auth/username`, { 
      newUsername 
    });
  }

  // Password Change
  updatePassword(currentPassword: string, newPassword: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.API_URL}/auth/password`, { 
      currentPassword, newPassword 
    });
  }

  // Project Publish/Unpublish
  publishProject(id: number): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/portfolio/projects/${id}/publish`, {});
  }

  unpublishProject(id: number): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/portfolio/projects/${id}/unpublish`, {});
  }
}

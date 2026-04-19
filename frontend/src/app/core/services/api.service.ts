import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
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
import { ApiResponse, LoginApiResponse } from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly API_URL = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  //Delete file
  deleteFile(fileUrl: string): Observable<void> {
    const fileName = fileUrl.split('/').pop() || '';
    return this.http.delete<void>(`${this.API_URL}/upload/file/${fileName}`);
  }
  // Auth
  login(username: string, password: string): Observable<{ token: string }> {
    return this.http.post<LoginApiResponse>(`${this.API_URL}/auth/login`, { username, password })
      .pipe(map(response => {
        if (!response.success || !response.token) {
          throw new Error(response.message || 'Login failed');
        }
        return { token: response.token };
      }));
  }

  // Portfolio Config (Public)
  getPortfolioConfig(): Observable<PortfolioConfig> {
    return this.http.get<ApiResponse<PortfolioConfig>>(`${this.API_URL}/portfolio/config`)
      .pipe(map(response => response.data!));
  }

   getSkills(): Observable<SkillCategory[]> {
     return this.http.get<ApiResponse<SkillCategory[]>>(`${this.API_URL}/portfolio/skills`)
       .pipe(map(response => response.data || []));
   }
 
   // Dashboard Stats (Admin)
   getDashboardStats(): Observable<DashboardStats> {
     return this.http.get<ApiResponse<DashboardStats>>(`${this.API_URL}/portfolio/dashboard-stats`)
       .pipe(map(response => response.data!));
   }
 
   getProjects(): Observable<Project[]> {
    return this.http.get<ApiResponse<Project[]>>(`${this.API_URL}/portfolio/projects`)
      .pipe(map(response => response.data || []));
  }

  // Projects (Admin - All including drafts)
  getAllProjectsAdmin(page: number = 1, pageSize: number = 10): Observable<PaginatedResponse<Project>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<ApiResponse<PaginatedResponse<Project>>>(`${this.API_URL}/portfolio/admin/projects`, { params })
      .pipe(map(response => response.data || { items: [], totalCount: 0, totalPages: 0, page: 1, pageSize: 10 }));
  }

 
  // Projects (Admin)
  createProject(project: Partial<Project>): Observable<Project> {
    return this.http.post<Project>(`${this.API_URL}/portfolio/projects`, project);
  }

  updateProject(project: Project): Observable<Project> {
    return this.http.put<Project>(`${this.API_URL}/portfolio/projects/${project.id}`, project);
  }

  deleteProject(id: number): Observable<void> {
    return this.http.delete<ApiResponse>(`${this.API_URL}/portfolio/projects/${id}`)
      .pipe(map(() => undefined));
  }
  
  // Hero (Admin)
  getProfileImage(): Observable<string> {
    return this.http.get<ApiResponse<string>>(`${this.API_URL}/portfolio/ProfileImage`)
      .pipe(map(response => response.data || ''));
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
    return this.http.delete<ApiResponse>(`${this.API_URL}/portfolio/journey/${id}`)
      .pipe(map(() => undefined));
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
    return this.http.delete<ApiResponse>(`${this.API_URL}/portfolio/socials/${id}`)
      .pipe(map(() => undefined));
  }

  // Skills (Admin)
  createSkillCategory(category: Partial<SkillCategory>): Observable<SkillCategory> {
    return this.http.post<SkillCategory>(`${this.API_URL}/portfolio/skills/categories`, category);
  }

  updateSkillCategory(category: SkillCategory): Observable<SkillCategory> {
    return this.http.put<SkillCategory>(`${this.API_URL}/portfolio/skills/categories`, category);
  }

  deleteSkillCategory(id: number): Observable<void> {
    return this.http.delete<ApiResponse>(`${this.API_URL}/portfolio/skills/categories/${id}`)
      .pipe(map(() => undefined));
  }

  createSkill(skill: { name: string; level: number; categoryId: number }): Observable<any> {
    return this.http.post(`${this.API_URL}/portfolio/skills`, skill);
  }

  deleteSkill(id: number): Observable<void> {
    return this.http.delete<ApiResponse>(`${this.API_URL}/portfolio/skills/${id}`)
      .pipe(map(() => undefined));
  }

  updateSkill(skill: { id?: number; name: string; level: number; categoryId: number }): Observable<any> {
    return this.http.put(`${this.API_URL}/portfolio/skills`, skill);
  }

  // Messages (Public - Contact Form)
  sendMessage(message: CreateMessageRequest): Observable<{ message: string; id: number }> {
    return this.http.post<ApiResponse<{ id: number }>>(`${this.API_URL}/messages`, message)
      .pipe(map(response => ({
        message: response.message,
        id: response.data?.id || 0
      })));
  }

  // Messages (Admin)
  getMessages(page: number = 1, pageSize: number = 20, isRead?: boolean): Observable<PaginatedResponse<Message>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (isRead !== undefined) {
      params = params.set('isRead', isRead.toString());
    }
    
    return this.http.get<ApiResponse<PaginatedResponse<Message>>>(`${this.API_URL}/messages`, { params })
      .pipe(map(response => response.data || { items: [], totalCount: 0, totalPages: 0, page: 1, pageSize: 20 }));
  }

  getMessage(id: number): Observable<Message> {
    return this.http.get<ApiResponse<Message>>(`${this.API_URL}/messages/${id}`)
      .pipe(map(response => response.data!));
  }

   markMessageAsRead(id: number): Observable<void> {
     return this.http.put<ApiResponse>(`${this.API_URL}/messages/${id}/read`, {})
       .pipe(map(() => undefined));
   }

   markAllMessagesAsRead(): Observable<{ markedAsReadCount: number }> {
     return this.http.put<ApiResponse<number>>(`${this.API_URL}/messages/read-all`, {})
       .pipe(map(response => ({
         markedAsReadCount: response.data || 0
       })));
   }

   deleteMessage(id: number): Observable<void> {
     return this.http.delete<ApiResponse>(`${this.API_URL}/messages/${id}`)
       .pipe(map(() => undefined));
   }

   // Reply to message
   respondToMessage(id: number, content: string): Observable<{ message: string }> {
     return this.http.post<ApiResponse>(`${this.API_URL}/messages/${id}/respond`, { content })
       .pipe(map(response => ({
         message: response.message
       })));
   }

   getUnreadCount(): Observable<number> {
    return this.http.get<ApiResponse<number>>(`${this.API_URL}/messages/unread-count`)
      .pipe(map(response => response.data || 0));
  }

  // File Upload
  uploadProjectImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<string>>(`${this.API_URL}/upload/project-image`, formData)
      .pipe(map(response => ({ url: response.data || '' })));
  }

  uploadProfileImage(file: File): Observable<{ data: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<string>>(`${this.API_URL}/upload/profile-image`, formData)
      .pipe(map(response => ({ data: response.data || '' })));
  }

  // Settings
  getSettings(category?: string): Observable<SystemSetting[]> {
    let params = new HttpParams();
    if (category) params = params.set('category', category);
    return this.http.get<ApiResponse<SystemSetting[]>>(`${this.API_URL}/settings`, { params })
      .pipe(map(response => response.data || []));
  }

  updateSetting(key: string, value: string, dataType: string = 'string'): Observable<void> {
    return this.http.put<ApiResponse>(`${this.API_URL}/settings`, { key, value, dataType })
      .pipe(map(() => undefined));
  }

  // Username Change
  updateUsername(newUsername: string): Observable<{ message: string }> {
    return this.http.put<ApiResponse>(`${this.API_URL}/auth/username`, { 
      newUsername 
    }).pipe(map(response => ({ message: response.message })));
  }

  // Password Change
  updatePassword(currentPassword: string, newPassword: string): Observable<{ message: string }> {
    return this.http.put<ApiResponse>(`${this.API_URL}/auth/password`, { 
      currentPassword, newPassword 
    }).pipe(map(response => ({ message: response.message })));
  }

 
}

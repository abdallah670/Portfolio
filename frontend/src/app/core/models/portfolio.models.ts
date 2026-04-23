// ============================================================
// PORTFOLIO CONFIG — matches GET /api/portfolio/config exactly
// ============================================================

export interface PortfolioConfig {
  hero: HeroConfig;
  skills: SkillCategoryConfig[];
  featuredProjects: ProjectConfig[];
  moreProjects: ProjectConfig[];
  journey: JourneyItemConfig[];
  socials: SocialLinkConfig[];
  contact: ContactConfig;
}

export interface HeroConfig {
  name: string;
  headlineTop: string;
  headlineMain: string;
  availabilityLabel: string;
  subtitle: string;
  profileImage: string;
  stats: HeroStatConfig[];
}

export interface HeroStatConfig {
  label: string;
  value: string;
}

export interface SkillCategoryConfig {
  id?: number;
  title: string; // NOTE: "title" not "name"
  color: string;
  skills: SkillConfig[];
}

export interface SkillConfig {
  id?: number;
  name: string;  // NOTE: "name" not "label"
  level: number;
}

export interface ProjectConfig {
  id: number;
  title: string;
  year: string;
  category: string;
  description: string;
  stack: string;       // raw JSON string e.g. "[\"C#\",\"Angular\"]"
  image: string;
  linkedinUrl: string;
  liveUrl :string;
  githubUrl: string;
  status: string;
  color: string;
  isFeatured: boolean;
  viewsCount?: number;
}

export interface JourneyItemConfig {
  id: number;
  title: string;
  period: string;
  org: string;         // NOTE: "org" not "organization"
  description: string;
}

export interface SocialLinkConfig {
  label: string;
  href: string;
  icon: string;
}

export interface ContactConfig {
  email: string;
  whatsApp: string;    // NOTE: capital A
  phone: string;
  location: string;
}

// ============================================================
// ADMIN — models for admin pages
// ============================================================

export interface Project {
  id: number;
  title: string;
  description: string;
  category: string;
  year: string;
  status: string;
  image: string;
  linkedinUrl: string;
  githubUrl: string;
  liveUrl: string;
  isFeatured: boolean;
  isPublished:boolean;
  color: string;
  stack: string;       // raw JSON string
  displayOrder: number;
  viewsCount?: number;
}

export interface Message {
  id: number;
  name: string;
  email: string;
  subject: string;
  preview?: string;
  content: string;
  isRead: boolean;
  createdAt: string;
  readAt?: string;
  isReplied: boolean;
  repliedAt?: string;
  replyContent?: string;
}

export interface Skill {
  id: number;
  name: string;
  level: number;
  categoryId: number;
}

export interface SkillCategory {
  id: number;
  title: string;
  color: string;
  displayOrder: number;
  skills: Skill[];
}

export interface Hero {
  id: number;
  name:string;
  headlineTop: string;
  headlineMain: string;
  subtitle: string;
  availabilityLabel: string;
 
  profileImage: string;
  stats: HeroStatConfig[];
}

export interface JourneyItem {
  id: number;
  title: string;
  org: string;         // NOTE: "org" not "organization"
  period: string;
  description: string;
  displayOrder: number;
}

export interface Contact {
  id: number;
  email: string;
  phone: string;
  whatsApp: string;    // NOTE: capital A
  location: string;
}

export interface SocialLink {
  id: number;
  label: string;
  href: string;
  icon: string;
  displayOrder: number;
}

// Dashboard Stats — matches GET /api/portfolio/dashboard-stats (camelCase JSON from .NET)
export interface DashboardStats {
  totalProjects: number;
  draftProjects: number;
  totalMessages: number;
  unreadMessages: number;
  repliedMessages: number;
  totalSkills: number;
  skillCategories: number;
  profileViews: number;
  recentProjects: { id: number; title: string; description: string; stack: string; status: string; image: string; year: string; category: string }[];
  projectsByMonth: { month: string; count: number }[];
  messagesByMonth: { month: string; count: number }[];
  viewsByMonth: { name: string; views: number }[];
}

// Paginated Response
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Contact Form Request
export interface CreateMessageRequest {
  name: string;
  email: string;
  subject?: string;
  content: string;
}

// System Setting
export interface SystemSetting {
  key: string;
  value: string;
  dataType: string;
  category: string;
}


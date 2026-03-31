// Project Model
export interface Project {
  id: number;
  title: string;
  description: string;
  shortDescription?: string;
  category: string;
  year: string;
  status: 'Published' | 'Draft';
  image: string;
  liveUrl: string;
  githubUrl: string;
  isFeatured: boolean;
  isPublished: boolean;
  viewsCount: number;
  color: string;
  stack: string;
  displayOrder: number;
}

// System Setting Model
export interface SystemSetting {
  id: number;
  key: string;
  value: string;
  dataType: 'string' | 'int' | 'bool' | 'json';
  category: string;
  description: string;
  updatedAt: string;
  updatedBy?: string;
}

// Analytics Models
export interface DashboardAnalytics {
  totalVisitors: number;
  totalPageViews: number;
  bounceRate: number;
  averageSessionDuration: string;
  trafficTrends: TrafficTrend[];
  topProjects: ProjectView[];
  topLocations: GeoLocation[];
  deviceBreakdown: DeviceBreakdown[];
}

export interface TrafficTrend {
  date: string;
  visitors: number;
  pageViews: number;
}

export interface ProjectView {
  projectId: number;
  projectName: string;
  views: number;
  percentage: number;
}

export interface GeoLocation {
  country: string;
  city: string;
  visitorCount: number;
  percentage: number;
}

export interface DeviceBreakdown {
  deviceType: string;
  count: number;
  percentage: number;
}

// Message Model
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
}

// Skill Model
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

// Hero Model
export interface Hero {
  id: number;
  headlineTop: string;
  headlineMain: string;
  subtitle: string;
  availabilityLabel: string;
  intro: string;
  ctaPrimaryLabel: string;
  ctaPrimaryHref: string;
  ctaSecondaryLabel: string;
  ctaSecondaryHref: string;
  profileImage: string;
  stats: HeroStat[];
}

export interface HeroStat {
  id: number;
  label: string;
  value: string;
  heroId: number;
}

// About Model
export interface About {
  id: number;
  kicker: string;
  title: string;
  subtitle: string;
  funFact: string;
  cards: AboutCard[];
  achievements: Achievement[];
  values: Value[];
}

export interface AboutCard {
  id: number;
  title: string;
  subtitle: string;
  aboutId: number;
}

export interface Achievement {
  id: number;
  text: string;
  aboutId: number;
}

export interface Value {
  id: number;
  title: string;
  description: string;
  aboutId: number;
}

// Journey Model
export interface JourneyItem {
  id: number;
  title: string;
  organization: string;
  period: string;
  description: string;
  displayOrder: number;
}

// Contact Model
export interface Contact {
  id: number;
  email: string;
  phone: string;
  whatsapp: string;
  location: string;
}

export interface SocialLink {
  id: number;
  label: string;
  href: string;
  icon: string;
  displayOrder: number;
}

// Dashboard Stats
export interface DashboardStats {
  totalProjects: number;
  draftProjects: number;
  totalMessages: number;
  unreadMessages: number;
  totalSkills: number;
  skillCategories: number;
  profileViews: number;
  recentProjects: Project[];
}

// Paginated Response
export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Create Message Request
export interface CreateMessageRequest {
  name: string;
  email: string;
  subject?: string;
  content: string;
}
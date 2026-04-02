// ============================================================
// PORTFOLIO CONFIG — matches GET /api/portfolio/config exactly
// ============================================================

export interface PortfolioConfig {
  hero: HeroConfig;
  about: AboutConfig;
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
  heroIntro: string;
  ctaPrimaryLabel: string;
  ctaPrimaryHref: string;
  ctaSecondaryLabel: string;
  ctaSecondaryHref: string;
  profileImage: string;
  stats: HeroStatConfig[];
}

export interface HeroStatConfig {
  label: string;
  value: string;
}

export interface AboutConfig {
  kicker: string;
  title: string;
  subtitle: string;
  funFact: string;
  cards: AboutCardConfig[];
  achievements: string[];
  values: ValueConfig[];
}

export interface AboutCardConfig {
  title: string;
  subtitle: string;
}

export interface ValueConfig {
  title: string;
  description: string; // NOTE: "description" not "desc"
}

export interface SkillCategoryConfig {
  title: string; // NOTE: "title" not "name"
  color: string;
  skills: SkillConfig[];
}

export interface SkillConfig {
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
  liveUrl: string;
  githubUrl: string;
  status: string;
  color: string;
  isFeatured: boolean;
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
  liveUrl: string;
  githubUrl: string;
  isFeatured: boolean;
  color: string;
  stack: string;       // raw JSON string
  displayOrder: number;
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
  headlineTop: string;
  headlineMain: string;
  subtitle: string;
  availabilityLabel: string;
  heroIntro: string;
  ctaPrimaryLabel: string;
  ctaPrimaryHref: string;
  ctaSecondaryLabel: string;
  ctaSecondaryHref: string;
  profileImage: string;
  stats: HeroStatConfig[];
}

export interface About {
  id: number;
  kicker: string;
  title: string;
  subtitle: string;
  funFact: string;
  cards: AboutCardConfig[];
  achievements: Achievement[];
  values: Value[];
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

// Dashboard Stats — matches GET /api/portfolio/dashboard-stats
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

// Analytics
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
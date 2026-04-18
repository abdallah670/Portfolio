import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { HeroConfig, SkillCategoryConfig, ContactConfig, SocialLinkConfig, Hero, Contact, SocialLink } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  activeTab: 'profile' | 'skills' | 'config' = 'profile';
  
  hero: HeroConfig | null = null;
  skills: SkillCategoryConfig[] = [];
  contact: ContactConfig | null = null;
  socials: SocialLinkConfig[] = [];
  
  passwordForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
  
  saving = false;
  message = '';
  success = false;
  loading = true;

  // CV Upload
  cvUploading = false;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.loading = true;
    this.api.getPortfolioConfig().subscribe({
      next: (config) => {
        this.hero = config.hero ? { ...config.hero } : this.getDefaultHero();
        this.skills = config.skills || [];
        this.contact = config.contact ? { ...config.contact } : this.getDefaultContact();
        this.socials = config.socials || [];
        this.loading = false;
      },
      error: () => {
        this.hero = this.getDefaultHero();
        this.skills = [];
        this.contact = this.getDefaultContact();
        this.socials = [];
        this.loading = false;
      }
    });
  }

  private getDefaultHero(): HeroConfig {
    return {
      name: 'Abdullah Mohammed',
      headlineTop: 'Hi, I\'m',
      headlineMain: 'Abdullah\nMohammed',
      subtitle: 'Full-Stack .NET Developer',
      availabilityLabel: 'Available for Opportunities',
      profileImage: '',
      stats: []
    };
  }

  private getDefaultContact(): ContactConfig {
    return {
      email: 'meno.mo.dev@gmail.com',
      whatsApp: '+201205450824',
      phone: '+201205450824',
      location: 'Cairo, Egypt'
    };
  }

  setTab(tab: 'profile' | 'skills' | 'config'): void {
    this.activeTab = tab;
  }

  saveHero(): void {
    if (!this.hero) return;
    this.saving = true;
    this.message = '';
    
    const heroData: Hero = {
      id: 1,
      name:this.hero.name,
      headlineTop: this.hero.headlineTop || '',
      headlineMain: this.hero.headlineMain || '',
      subtitle: this.hero.subtitle || '',
      availabilityLabel: this.hero.availabilityLabel || '',
      profileImage: this.hero.profileImage || '',
      stats: this.hero.stats || []
    };
    
    this.api.updateHero(heroData).subscribe({
      next: () => {
        this.message = 'Profile updated successfully';
        this.success = true;
        this.saving = false;
      },
      error: () => {
        this.message = 'Failed to update profile';
        this.success = false;
        this.saving = false;
      }
    });
  }

  onProfileImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.api.uploadProfileImage(file).subscribe({
        next: (res) => {
          if (this.hero) this.hero.profileImage = res.url;
        },
        error: () => {
          this.message = 'Failed to upload image';
          this.success = false;
        }
      });
    }
  }

  getImageUrl(path: string | undefined): string {
    if (!path) return '';
    if (path.startsWith('http')) return path;
    return `http://localhost:5000/${path}`;
  }

  addStat(): void {
    if (!this.hero) return;
    this.hero.stats = this.hero.stats || [];
    this.hero.stats.push({ value: '0', label: 'New Stat' });
  }

  removeStat(index: number): void {
    if (!this.hero) return;
    this.hero.stats?.splice(index, 1);
  }

  addSkillCategory(): void {
    const newCategory: SkillCategoryConfig = {
      title: 'New Category',
      color: 'blue',
      skills: []
    };
    this.skills.push(newCategory);
  }

  addSkillToCategory(categoryIndex: number): void {
    const category = this.skills[categoryIndex];
    category.skills = category.skills || [];
    category.skills.push({ name: 'New Skill', level: 50 });
  }

  removeSkill(categoryIndex: number, skillIndex: number): void {
    this.skills[categoryIndex].skills?.splice(skillIndex, 1);
  }

  removeCategory(index: number): void {
    this.skills.splice(index, 1);
  }

  addSocial(): void {
    this.socials.push({ label: 'New Link', href: 'https://', icon: 'link' });
  }

  removeSocial(index: number): void {
    this.socials.splice(index, 1);
  }

  saveContact(): void {
    if (!this.contact) return;
    this.saving = true;
    this.message = '';
    
    const contactData: Contact = {
      id: 1,
      email: this.contact.email || '',
      whatsApp: this.contact.whatsApp || '',
      phone: this.contact.phone || '',
      location: this.contact.location || ''
    };
    
    this.api.updateContact(contactData).subscribe({
      next: () => {
        this.message = 'Contact info updated successfully';
        this.success = true;
        this.saving = false;
      },
      error: () => {
        this.message = 'Failed to update contact info';
        this.success = false;
        this.saving = false;
      }
    });
  }

  saveSocials(): void {
    this.saving = true;
    this.message = '';
    let completed = 0;
    const total = this.socials.length;

    if (total === 0) {
      this.saving = false;
      return;
    }

    this.socials.forEach((socialConfig, index) => {
      const social: SocialLink = {
        id: index + 1,
        label: socialConfig.label || '',
        href: socialConfig.href || '',
        icon: socialConfig.icon || 'link',
        displayOrder: index
      };
      
      this.api.updateSocial(social).subscribe({
        next: () => {
          completed++;
          if (completed === total) {
            this.message = 'Social links updated successfully';
            this.success = true;
            this.saving = false;
          }
        },
        error: () => {
          completed++;
          if (completed === total) {
            this.message = 'Failed to update some social links';
            this.success = false;
            this.saving = false;
          }
        }
      });
    });
  }

  changePassword(): void {
    if (!this.passwordForm.currentPassword || !this.passwordForm.newPassword) {
      this.message = 'Please fill all password fields';
      this.success = false;
      return;
    }

    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.message = 'New passwords do not match';
      this.success = false;
      return;
    }

    if (this.passwordForm.newPassword.length < 6) {
      this.message = 'Password must be at least 6 characters';
      this.success = false;
      return;
    }

    this.saving = true;
    this.message = '';

    this.api.updatePassword(this.passwordForm.currentPassword, this.passwordForm.newPassword).subscribe({
      next: () => {
        this.message = 'Password updated successfully';
        this.success = true;
        this.passwordForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
        this.saving = false;
      },
      error: (err) => {
        this.message = err.error?.message || 'Failed to update password';
        this.success = false;
        this.saving = false;
      }
    });
  }

  onCVSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file type
      if (file.type !== 'application/pdf') {
        this.message = 'Only PDF files are allowed';
        this.success = false;
        return;
      }
      
      // Validate file size (10MB max)
      if (file.size > 10 * 1024 * 1024) {
        this.message = 'File too large. Max 10MB allowed.';
        this.success = false;
        return;
      }

      this.cvUploading = true;
      this.message = '';
      
      this.api.uploadCV(file).subscribe({
        next: (res) => {
          this.message = 'CV uploaded successfully';
          this.success = true;
          this.cvUploading = false;
          // Reset the input
          input.value = '';
        },
        error: (err) => {
          this.message = err.error?.message || 'Failed to upload CV';
          this.success = false;
          this.cvUploading = false;
          input.value = '';
        }
      });
    }
  }
}

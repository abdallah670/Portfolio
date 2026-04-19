import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { SweetAlertService } from '../../../core/services/sweetalert.service';
import { UserService } from '../../../core/services/user.service';
import { HeroConfig, SkillCategoryConfig, ContactConfig, SocialLinkConfig, Hero, Contact, SocialLink, JourneyItem, SkillCategory } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  activeTab: 'profile' | 'skills' | 'config' | 'journey' = 'profile';
  
  hero: HeroConfig | null = null;
  skills: SkillCategoryConfig[] = [];
  contact: ContactConfig | null = null;
  socials: SocialLinkConfig[] = [];
  journey: JourneyItem[] = [];
  
  passwordForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
  usernameForm = { newUsername: '' };
  
  saving = false;
  updatingUsername = false;
  message = '';
  success = false;
  loading = true;

  // CV Upload
  cvUploading = false;
  savingCategory: number | null = null;

  constructor(private api: ApiService, private sweetAlert: SweetAlertService, private userService: UserService) {}

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
    this.loadJourney();
  }

   loadJourney(): void {
     this.api.getJourney().subscribe({
       next: (journey) => {
         this.journey = journey;
       },
       error: () => {
         this.journey = [];
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

  setTab(tab: 'profile' | 'skills' | 'config' | 'journey'): void {
    this.activeTab = tab;
  }

  saveHero(): void {
    if (!this.hero) return;
    this.saving = true;
    this.message = '';
    
    // Extract stats from hero config
    const stats = this.hero.stats?.map((s, index) => ({
      id: 0,
      label: s.label,
      value: s.value,
      displayOrder: index
    })) || [];
    
    const heroData: Hero = {
      id: 1,
      name: this.hero.name,
      headlineTop: this.hero.headlineTop || '',
      headlineMain: this.hero.headlineMain || '',
      subtitle: this.hero.subtitle || '',
      availabilityLabel: this.hero.availabilityLabel || '',
      profileImage: this.hero.profileImage || '',
      stats: [] // Stats sent separately
    };
    
    this.api.updateHero(heroData, stats).subscribe({
      next: () => {
        this.saving = false;
        this.sweetAlert.success('Profile Updated', 'Your profile has been saved successfully.');
      },
      error: () => {
        this.saving = false;
        this.sweetAlert.error('Error', 'Failed to update profile.');
      }
    });
  }

  onProfileImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.uploadProfileImage(file);
    
     
       
    }
  }
  uploadProfileImage(file: File): void {
      this.api.uploadProfileImage(file).subscribe({
           next: (res) => {
          if (this.hero && res.data) {
            // Add cache-busting timestamp to force browser to reload image
            const cacheBuster = `?t=${Date.now()}`;
            this.hero.profileImage = res.data + cacheBuster;
            // Update header image immediately
            this.userService.setProfileImage(this.hero.profileImage);
          }
          this.sweetAlert.success('Image Uploaded', 'Profile image has been updated.');
           },
            error: () => {
          this.sweetAlert.error('Error', 'Failed to upload image.');
            }
         });
  }

  getImageUrl(path: string | undefined): string {
    if (!path) return '';
    if (path.startsWith('http')) return path;
    // Handle cache-buster query string - don't prepend if it starts with /
    if (path.startsWith('/')) return `http://localhost:5000${path}`;
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
    const skill = this.skills[categoryIndex].skills?.[skillIndex];
    if (!skill) return;
    
    this.sweetAlert.deleteConfirm(skill.name).then((confirmed) => {
      if (confirmed) {
        this.skills[categoryIndex].skills?.splice(skillIndex, 1);
        this.sweetAlert.success('Deleted', 'Skill removed successfully.');
      }
    });
  }

  removeCategory(index: number): void {
    const category = this.skills[index];
    if (!category) return;
    
    this.sweetAlert.deleteConfirm(category.title).then((confirmed) => {
      if (confirmed) {
        if (category.id) {
          this.api.deleteSkillCategory(category.id).subscribe({
            next: () => {
              this.skills.splice(index, 1);
              this.sweetAlert.success('Deleted', 'Category deleted successfully.');
            },
            error: () => {
              this.sweetAlert.error('Error', 'Failed to delete category.');
            }
          });
        } else {
          this.skills.splice(index, 1);
          this.sweetAlert.success('Deleted', 'Category removed successfully.');
        }
      }
    });
  }

  saveCategory(index: number): void {
    const catConfig = this.skills[index];
    this.savingCategory = index;

    const category: SkillCategory = {
      id: catConfig.id || 0,
      title: catConfig.title || 'Untitled',
      color: catConfig.color || 'blue',
      displayOrder: index,
      skills: (catConfig.skills || []).map(s => ({
        id: s.id || 0,
        name: s.name || 'Untitled',
        level: s.level || 50,
        categoryId: catConfig.id || 0
      }))
    };

    const request = category.id
      ? this.api.updateSkillCategory(category)
      : this.api.createSkillCategory(category);

    request.subscribe({
      next: (saved) => {
        if (!catConfig.id && saved.id) {
          catConfig.id = saved.id;
        }
        if (saved.skills) {
          saved.skills.forEach((savedSkill, i) => {
            if (catConfig.skills?.[i] && !catConfig.skills[i].id) {
              catConfig.skills[i].id = savedSkill.id;
            }
          });
        }
        this.savingCategory = null;
        this.sweetAlert.success('Saved', `${catConfig.title} updated.`);
      },
      error: () => {
        this.savingCategory = null;
        this.sweetAlert.error('Error', 'Failed to save category.');
      }
    });
  }

  saveSkills(): void {
    this.saving = true;
    this.message = '';
    let completed = 0;
    const total = this.skills.length;

    if (total === 0) {
      this.saving = false;
      return;
    }

    this.skills.forEach((catConfig, index) => {
      const category: SkillCategory = {
        id: catConfig.id || 0,
        title: catConfig.title || 'Untitled',
        color: catConfig.color || 'blue',
        displayOrder: index,
        skills: (catConfig.skills || []).map(s => ({
          id: s.id || 0, // Preserve existing skill ID
          name: s.name || 'Untitled',
          level: s.level || 50,
          categoryId: catConfig.id || 0
        }))
      };

      const request = category.id ? this.api.updateSkillCategory(category) : this.api.createSkillCategory(category);

      request.subscribe({
        next: (savedCategory) => {
          completed++;
          if (catConfig.id === 0 && savedCategory.id) {
            catConfig.id = savedCategory.id;
          }
          if (completed === total) {
            this.saving = false;
            this.sweetAlert.success('Skills Updated', 'Skills have been saved successfully.');
            this.loadAllData();
          }
        },
        error: () => {
          completed++;
          if (completed === total) {
            this.saving = false;
            this.sweetAlert.error('Error', 'Failed to save some skill categories.');
          }
        }
      });
    });
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
        this.saving = false;
        this.sweetAlert.success('Contact Updated', 'Contact information has been saved successfully.');
      },
      error: () => {
        this.saving = false;
        this.sweetAlert.error('Error', 'Failed to update contact info. Please try again.');
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
            this.saving = false;
            this.sweetAlert.success('Social Links Updated', 'Social links have been saved successfully.');
          }
        },
        error: () => {
          completed++;
          if (completed === total) {
            this.saving = false;
            this.sweetAlert.error('Error', 'Failed to update some social links.');
          }
        }
      });
    });
  }

  changeUsername(): void {
    if (!this.usernameForm.newUsername || this.usernameForm.newUsername.trim().length < 3) {
      this.sweetAlert.warning('Invalid Username', 'Username must be at least 3 characters.');
      return;
    }

    this.updatingUsername = true;

    this.api.updateUsername(this.usernameForm.newUsername.trim()).subscribe({
      next: () => {
        this.updatingUsername = false;
        this.sweetAlert.success('Username Updated', 'Please log in again.');
        setTimeout(() => {
          localStorage.removeItem('token');
          window.location.href = '/admin/login';
        }, 2000);
      },
      error: (err) => {
        this.updatingUsername = false;
        this.sweetAlert.error('Error', err.error?.errors?.[0] || 'Failed to update username.');
      }
    });
  }

  changePassword(): void {
    if (!this.passwordForm.currentPassword || !this.passwordForm.newPassword) {
      this.sweetAlert.warning('Missing Fields', 'Please fill all password fields.');
      return;
    }

    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.sweetAlert.warning('Mismatch', 'New passwords do not match.');
      return;
    }

    if (this.passwordForm.newPassword.length < 6) {
      this.sweetAlert.warning('Weak Password', 'Password must be at least 6 characters.');
      return;
    }

    this.saving = true;

    this.api.updatePassword(this.passwordForm.currentPassword, this.passwordForm.newPassword).subscribe({
      next: () => {
        this.saving = false;
        this.sweetAlert.success('Password Updated', 'Your password has been changed.');
        this.passwordForm = { currentPassword: '', newPassword: '', confirmPassword: '' };
      },
      error: (err) => {
        this.saving = false;
        this.sweetAlert.error('Error', err.error?.message || 'Failed to update password.');
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
        next: () => {
          this.sweetAlert.success('CV Uploaded', 'Your CV has been uploaded successfully.');
          this.cvUploading = false;
          input.value = '';
        },
        error: (err) => {
          this.sweetAlert.error('Error', err.error?.message || 'Failed to upload CV.');
          this.cvUploading = false;
          input.value = '';
        }
      });
    }
   }
   
   // Journey
   addJourneyItem(): void {
     const newItem: JourneyItem = {
       id: 0,
       title: 'New Journey Item',
       period: '2024 - Present',
       org: 'Company Name',
       description: '',
       displayOrder: this.journey.length
     };
     this.journey.push(newItem);
   }

removeJourneyItem(index: number): void {
      const item = this.journey[index];
      this.sweetAlert.deleteConfirm(item.title).then((confirmed) => {
        if (confirmed) {
          if (item.id && item.id !== 0) {
            this.api.deleteJourney(item.id).subscribe({
              next: () => {
                this.journey.splice(index, 1);
                this.sweetAlert.success('Deleted', 'Journey item deleted successfully.');
              },
              error: () => this.sweetAlert.error('Failed', 'Could not delete journey item.')
            });
          } else {
            this.journey.splice(index, 1);
          }
        }
      });
    }

   saveJourney(): void {
     this.saving = true;
     this.message = '';
     const items = this.journey.filter(item => item.title && item.period && item.org);
     let completed = 0;
     const total = items.length;

     if (total === 0) {
       this.saving = false;
       return;
     }

     items.forEach((item) => {
const payload: Partial<JourneyItem> = {
          title: item.title,
          period: item.period,
          org: item.org,
          description: item.description || '',
          displayOrder: this.journey.indexOf(item)
        };
        if (item.id) {
          payload.id = item.id;
        }

        const request = item.id ? this.api.updateJourney(payload as JourneyItem) : this.api.createJourney(payload);

request.subscribe({
          next: () => {
            completed++;
            if (completed === total) {
              this.saving = false;
              this.sweetAlert.success('Journey Updated', 'Journey items have been saved successfully.');
              this.loadAllData();
            }
          },
          error: () => {
            completed++;
            if (completed === total) {
              this.saving = false;
              this.sweetAlert.error('Error', 'Failed to save some journey items.');
            }
          }
        });
     });
   }

   private setMessage(msg: string, success: boolean): void {
     this.message = msg;
     this.success = success;
   }
 }

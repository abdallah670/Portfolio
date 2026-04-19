import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { SweetAlertService } from '../../../core/services/sweetalert.service';
import { SkillCategory, Skill } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="skills-page">
      <header class="page-header">
        <div>
          <h1>Skills & Tools</h1>
          <p class="subtitle">Manage your skill categories and individual skills</p>
        </div>
        <button class="btn btn-primary" (click)="addCategory()">
          <span class="material-symbols-outlined">add</span>
          Add Category
        </button>
      </header>

      @if (loading) {
        <div class="loading">
          <div class="spinner"></div>
          <p>Loading skills...</p>
        </div>
      }

      @if (!loading) {
        <div class="categories-grid">
          @for (category of categories; track category.id; let catIdx = $index) {
            <div class="category-card" [attr.data-color]="category.color">
              <div class="category-header">
                <div class="category-title-row">
                  <input
                    type="text"
                    [(ngModel)]="category.title"
                    [name]="'catTitle' + catIdx"
                    class="title-input"
                    placeholder="Category name"
                    (blur)="saveCategory(category)"
                  />
                  <div class="category-actions">
                    <select
                      [(ngModel)]="category.color"
                      [name]="'catColor' + catIdx"
                      class="color-select"
                      (change)="saveCategory(category)"
                    >
                      <option value="blue">Blue</option>
                      <option value="emerald">Emerald</option>
                      <option value="purple">Purple</option>
                      <option value="orange">Orange</option>
                      <option value="red">Red</option>
                      <option value="pink">Pink</option>
                      <option value="teal">Teal</option>
                      <option value="cyan">Cyan</option>
                    </select>
                    <button class="btn-icon" (click)="deleteCategory(category)">
                      <span class="material-symbols-outlined">delete</span>
                    </button>
                  </div>
                </div>
              </div>
              
              <div class="skills-list">
                @for (skill of category.skills; track skill.id; let skillIdx = $index) {
                  <div class="skill-row">
                    <input
                      type="text"
                      [(ngModel)]="skill.name"
                      [name]="'skillName' + catIdx + '_' + skillIdx"
                      class="skill-name-input"
                      placeholder="Skill name"
                      (blur)="saveSkill(skill, category)"
                    />
                    <div class="skill-level-input">
                      <input
                        type="range"
                        [(ngModel)]="skill.level"
                        [name]="'skillLevel' + catIdx + '_' + skillIdx"
                        min="0"
                        max="100"
                        (change)="saveSkill(skill, category)"
                      />
                      <span class="level-value">{{ skill.level }}%</span>
                    </div>
                    <button class="btn-icon small" (click)="deleteSkill(skill, category)">
                      <span class="material-symbols-outlined">close</span>
                    </button>
                  </div>
                }
                @empty {
                  <p class="empty-skills">No skills in this category</p>
                }
              </div>

              <button class="btn btn-ghost add-skill-btn" (click)="addSkill(category)">
                <span class="material-symbols-outlined">add</span>
                Add Skill
              </button>
            </div>
          }
          @empty {
            <div class="empty-state">
              <span class="material-symbols-outlined">psychology</span>
              <h3>No skill categories yet</h3>
              <p>Click "Add Category" to create your first category</p>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .skills-page {
      padding: 24px;
    }
    
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 32px;
    }
    
    h1 {
      font-size: 28px;
      font-weight: 700;
      margin: 0;
      color: var(--foreground);
    }
    
    .subtitle {
      color: var(--muted-foreground);
      margin-top: 4px;
    }
    
    .loading {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 48px;
      color: var(--muted-foreground);
    }
    
    .categories-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 24px;
    }
    
    .category-card {
      background: var(--card);
      border: 1px solid var(--border);
      border-radius: 12px;
      overflow: hidden;
    }
    
    .category-card[data-color="blue"] { border-top: 3px solid #3b82f6; }
    .category-card[data-color="emerald"] { border-top: 3px solid #10b981; }
    .category-card[data-color="purple"] { border-top: 3px solid #8b5cf6; }
    .category-card[data-color="orange"] { border-top: 3px solid #f97316; }
    .category-card[data-color="red"] { border-top: 3px solid #ef4444; }
    .category-card[data-color="pink"] { border-top: 3px solid #ec4899; }
    .category-card[data-color="teal"] { border-top: 3px solid #14b8a6; }
    .category-card[data-color="cyan"] { border-top: 3px solid #06b6d4; }
    
    .category-header {
      padding: 16px;
      border-bottom: 1px solid var(--border);
    }
    
    .category-title-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 12px;
    }
    
    .title-input {
      flex: 1;
      background: transparent;
      border: none;
      font-size: 18px;
      font-weight: 600;
      color: var(--foreground);
      padding: 4px 0;
    }
    
    .title-input:focus {
      outline: none;
      border-bottom: 2px solid var(--primary);
    }
    
    .category-actions {
      display: flex;
      gap: 8px;
      align-items: center;
    }
    
    .color-select {
      background: var(--background);
      border: 1px solid var(--border);
      border-radius: 6px;
      padding: 4px 8px;
      color: var(--foreground);
      font-size: 12px;
    }
    
    .skills-list {
      padding: 16px;
      max-height: 300px;
      overflow-y: auto;
    }
    
    .skill-row {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 8px 0;
      border-bottom: 1px solid var(--border);
    }
    
    .skill-row:last-child {
      border-bottom: none;
    }
    
    .skill-name-input {
      background: transparent;
      border: none;
      font-weight: 500;
      color: var(--foreground);
      width: 120px;
      padding: 4px;
    }
    
    .skill-name-input:focus {
      outline: none;
      border-bottom: 2px solid var(--primary);
    }
    
    .skill-level-input {
      display: flex;
      align-items: center;
      gap: 8px;
      flex: 1;
    }
    
    .skill-level-input input[type="range"] {
      flex: 1;
      height: 6px;
      -webkit-appearance: none;
      background: var(--border);
      border-radius: 3px;
    }
    
    .skill-level-input input[type="range"]::-webkit-slider-thumb {
      -webkit-appearance: none;
      width: 14px;
      height: 14px;
      background: var(--primary);
      border-radius: 50%;
      cursor: pointer;
    }
    
    .level-value {
      font-size: 12px;
      color: var(--muted-foreground);
      min-width: 35px;
    }
    
    .skill-info {
      display: flex;
      justify-content: space-between;
      width: 100%;
      min-width: 120px;
    }
    
    .skill-name {
      font-weight: 500;
      color: var(--foreground);
    }
    
    .skill-level {
      color: var(--muted-foreground);
      font-size: 12px;
    }
    
    .skill-bar {
      flex: 1;
      height: 6px;
      background: var(--border);
      border-radius: 3px;
      overflow: hidden;
    }
    
    .skill-progress {
      height: 100%;
      background: var(--primary);
      border-radius: 3px;
      transition: width 0.3s ease;
    }
    
    .btn-icon {
      background: transparent;
      border: none;
      color: var(--muted-foreground);
      cursor: pointer;
      padding: 4px;
      border-radius: 4px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    
    .btn-icon:hover {
      background: var(--border);
      color: var(--foreground);
    }
    
    .btn-icon.small {
      padding: 2px;
    }
    
    .btn-icon.small .material-symbols-outlined {
      font-size: 18px;
    }
    
    .add-skill-btn {
      width: 100%;
      justify-content: center;
      border-top: 1px solid var(--border);
      border-radius: 0;
    }
    
    .empty-skills {
      color: var(--muted-foreground);
      font-size: 14px;
      text-align: center;
      padding: 16px;
    }
    
    .empty-state {
      grid-column: 1 / -1;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 64px;
      background: var(--card);
      border: 1px dashed var(--border);
      border-radius: 12px;
      text-align: center;
    }
    
    .empty-state .material-symbols-outlined {
      font-size: 48px;
      color: var(--muted-foreground);
      margin-bottom: 16px;
    }
    
    .empty-state h3 {
      margin: 0;
      color: var(--foreground);
    }
    
    .empty-state p {
      color: var(--muted-foreground);
      margin-top: 8px;
    }
  `]
})
export class SkillsComponent implements OnInit {
  categories: SkillCategory[] = [];
  loading = true;
  saving = false;

  constructor(
    private api: ApiService,
    private sweetAlert: SweetAlertService
  ) {}

  ngOnInit(): void {
    this.loadSkills();
  }

  loadSkills(): void {
    this.loading = true;
    this.api.getPortfolioConfig().subscribe({
      next: (config) => {
        this.categories = (config.skills || []).map((cat: any) => ({
          id: cat.id || 0,
          title: cat.title || '',
          color: cat.color || 'blue',
          displayOrder: 0,
          skills: (cat.skills || []).map((s: any) => ({
            id: s.id || 0,
            name: s.name || '',
            level: s.level || 50,
            categoryId: cat.id || 0
          }))
        }));
        this.loading = false;
      },
      error: () => {
        this.categories = [];
        this.loading = false;
      }
    });
  }

  addCategory(): void {
    const newCategory: SkillCategory = {
      id: 0,
      title: 'New Category',
      color: 'blue',
      displayOrder: this.categories.length,
      skills: []
    };
    this.categories.push(newCategory);
  }

  saveCategory(category: SkillCategory): void {
    if (!category.title) {
      this.sweetAlert.warning('Title Required', 'Please enter a category title.');
      return;
    }

    const request = category.id 
      ? this.api.updateSkillCategory(category)
      : this.api.createSkillCategory(category);

    request.subscribe({
      next: (saved) => {
        if (!category.id) {
          category.id = saved.id;
        }
        this.sweetAlert.success('Saved', 'Category saved successfully.');
      },
      error: () => {
        this.sweetAlert.error('Error', 'Failed to save category.');
      }
    });
  }

  deleteCategory(category: SkillCategory): void {
    this.sweetAlert.deleteConfirm(category.title).then((confirmed) => {
      if (confirmed) {
        if (category.id) {
          this.api.deleteSkillCategory(category.id).subscribe({
            next: () => {
              this.categories = this.categories.filter(c => c.id !== category.id);
              this.sweetAlert.success('Deleted', 'Category deleted successfully.');
            },
            error: () => {
              this.sweetAlert.error('Error', 'Failed to delete category.');
            }
          });
        } else {
          this.categories = this.categories.filter(c => c !== category);
        }
      }
    });
  }

  addSkill(category: SkillCategory): void {
    const newSkill: Skill = {
      id: 0,
      name: 'New Skill',
      level: 50,
      categoryId: category.id || 0
    };
    
    category.skills = category.skills || [];
    category.skills.push(newSkill);
    
    // If category has an id, also save the skill
    if (category.id) {
      this.api.createSkill({ name: newSkill.name, level: newSkill.level, categoryId: category.id }).subscribe({
        next: (saved) => {
          newSkill.id = saved.id;
          this.sweetAlert.success('Added', 'Skill added to category.');
        },
        error: () => {
          this.sweetAlert.error('Error', 'Failed to add skill.');
        }
      });
    }
  }

  saveSkill(skill: Skill, category: SkillCategory): void {
    if (!skill.name) {
      this.sweetAlert.warning('Name Required', 'Please enter a skill name.');
      return;
    }

    if (skill.id) {
      this.api.updateSkill({ id: skill.id, name: skill.name, level: skill.level, categoryId: category.id }).subscribe({
        next: () => {
          this.sweetAlert.success('Saved', 'Skill updated.');
        },
        error: () => {
          this.sweetAlert.error('Error', 'Failed to update skill.');
        }
      });
    } else if (category.id) {
      // New skill without id, create it
      this.api.createSkill({ name: skill.name, level: skill.level, categoryId: category.id }).subscribe({
        next: (saved) => {
          skill.id = saved.id;
          this.sweetAlert.success('Added', 'Skill added to category.');
        },
        error: () => {
          this.sweetAlert.error('Error', 'Failed to add skill.');
        }
      });
    }
  }

  deleteSkill(skill: Skill, category: SkillCategory): void {
    this.sweetAlert.deleteConfirm(skill.name).then((confirmed) => {
      if (confirmed) {
        if (skill.id) {
          this.api.deleteSkill(skill.id).subscribe({
            next: () => {
              category.skills = category.skills?.filter(s => s.id !== skill.id);
              this.sweetAlert.success('Deleted', 'Skill deleted successfully.');
            },
            error: () => {
              this.sweetAlert.error('Error', 'Failed to delete skill.');
            }
          });
        } else {
          category.skills = category.skills?.filter(s => s !== skill);
        }
      }
    });
  }
}
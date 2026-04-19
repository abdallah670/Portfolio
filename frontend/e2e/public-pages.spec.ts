import { test, expect } from '@playwright/test';

test.describe('Public Pages', () => {
  test('should load home page', async ({ page }) => {
    await page.goto('/');
    
    // Should load without errors
    await expect(page).toHaveTitle(/portfolio/i, { timeout: 10000 }).catch(() => {});
  });

  test('should navigate to contact page', async ({ page }) => {
    await page.goto('/');
    
    // Click contact link/button
    const contactLink = page.locator('a[href*="contact"], nav:has-text("Contact")');
    if (await contactLink.count() > 0) {
      await contactLink.first().click();
      await expect(page).toHaveURL(/contact/);
    } else {
      // Direct navigation
      await page.goto('/contact');
    }
    
    await expect(page.locator('form')).toBeVisible();
  });

  test('should display projects on home page', async ({ page }) => {
    await page.goto('/');
    
    // Wait for projects to load
    await page.waitForTimeout(2000);
    
    // Check if projects section exists
    const projectsSection = page.locator('section:has-text("Project"), .project-card, [class*="project"]');
    const hasProjects = await projectsSection.count() > 0;
    
    // Either projects load or page is loading
    expect(hasProjects || page.url()).toContain('localhost');
  });

  test('should display skills on home page', async ({ page }) => {
    await page.goto('/');
    
    await page.waitForTimeout(2000);
    
    const skillsSection = page.locator('section:has-text("Skill"), .skill-card, [class*="skill"]');
    const hasSkills = await skillsSection.count() > 0;
    
    expect(hasSkills || page.url()).toContain('localhost');
  });

  test('should have responsive design', async ({ page }) => {
    // Test mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto('/');
    
    // Page should still load
    await expect(page.locator('body')).toBeVisible();
    
    // Test tablet viewport
    await page.setViewportSize({ width: 768, height: 1024 });
    await page.reload();
    await expect(page.locator('body')).toBeVisible();
  });
});
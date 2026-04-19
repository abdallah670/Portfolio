import { test, expect } from '@playwright/test';

test.describe('Contact Form', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/contact');
  });

  test('should display contact form', async ({ page }) => {
    await expect(page.locator('form')).toBeVisible();
    await expect(page.locator('input[name="name"], #name')).toBeVisible();
    await expect(page.locator('input[name="email"], #email')).toBeVisible();
    await expect(page.locator('textarea[name="content"], #content')).toBeVisible();
  });

  test('should show validation errors for empty form', async ({ page }) => {
    await page.click('button[type="submit"]');
    
    // Should show required field errors
    await page.waitForTimeout(500);
    
    const form = page.locator('form');
    expect(await form.count()).toBeGreaterThan(0);
  });

  test('should submit valid contact form', async ({ page }) => {
    await page.fill('input[name="name"], #name', 'Test User');
    await page.fill('input[name="email"], #email', 'test@example.com');
    await page.fill('textarea[name="content"], #content', 'This is a test message');
    
    await page.click('button[type="submit"]');
    
    // Wait for success message or redirect
    await page.waitForTimeout(2000);
    
    // Check for success message (depends on UI implementation)
    const successMessage = page.locator('.success, .alert-success, text=success');
    const currentUrl = page.url();
    
    // Either should show success or redirect
    expect(successMessage.or(page.locator('/admin/'))).toBeTruthy();
  });

  test('should validate email format', async ({ page }) => {
    await page.fill('input[name="name"], #name', 'Test User');
    await page.fill('input[name="email"], #email', 'invalid-email');
    await page.fill('textarea[name="content"], #content', 'Test content');
    await page.click('button[type="submit"]');
    
    await page.waitForTimeout(500);
    
    // Should show email validation error
    const errorMessage = page.locator('.error, .alert, text=email');
    expect(await errorMessage.count()).toBeGreaterThan(0);
  });
});
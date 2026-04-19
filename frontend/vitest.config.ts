import { defineConfig } from 'vitest/config';
import { resolve } from 'path';

export default defineConfig({
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: ['src/test/setup.ts'],
    include: ['src/**/*.spec.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      exclude: [
        'node_modules/',
        'src/test/',
        '**/*.interface.ts',
        '**/*.type.ts',
        '**/models.ts',
        '**/environments/**',
      ],
    },
  },
  resolve: {
    alias: {
      '@app': resolve(__dirname, 'src/app'),
    },
  },
});
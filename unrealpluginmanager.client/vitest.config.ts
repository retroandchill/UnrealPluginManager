/// <reference types="vitest" />
import {defineConfig} from 'vite'
import react from '@vitejs/plugin-react'
import path from "path";

export default defineConfig({
  plugins: react(),
  test: {
    environment: "jsdom",
    setupFiles: ["./setupTests.ts"],
    coverage: {
      provider: 'istanbul',
      reporter: ['text', 'lcov'],
    }
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
})
﻿/// <reference types="vitest" />
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
    plugins: react(),
    test: {
        environment: "jsdom",
        setupFiles: ["./setupTests.ts"],
        coverage: {
            provider: 'v8',
            reporter: ['text', 'json', 'html'],
        }
    },
})
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';
// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        // React plugin enables Fast Refresh for instant HMR during development.
        react(),
    ],
    resolve: {
        alias: {
            // '@/' maps to src/ so all imports are absolute and refactor-safe.
            // Example: import { useAuth } from '@/shared/hooks/useAuth'
            '@': path.resolve(__dirname, './src'),
        },
    },
    server: {
        port: 3000,
        // Proxy API calls to the .NET backend during development.
        // This avoids CORS issues without changing browser origin.
        proxy: {
            '/api': {
                target: process.env.VITE_API_BASE_URL ?? 'http://localhost:5000',
                changeOrigin: true,
            },
            '/connect': {
                // OpenIddict token endpoint lives at /connect/token (not under /api/).
                target: process.env.VITE_API_BASE_URL ?? 'http://localhost:5000',
                changeOrigin: true,
            },
        },
    },
    build: {
        outDir: 'dist',
        sourcemap: true,
        // Split vendor chunks for better long-term caching.
        rollupOptions: {
            output: {
                manualChunks: {
                    react: ['react', 'react-dom'],
                    router: ['react-router-dom'],
                    query: ['@tanstack/react-query'],
                    // CoreUI is the CSS library (compiled via SCSS); only the JS icon
                    // package is chunked here.  Plain bootstrap was removed in favour
                    // of @coreui/coreui.
                    ui: ['react-bootstrap'],
                    coreui: ['@coreui/icons-react'],
                },
            },
        },
    },
});

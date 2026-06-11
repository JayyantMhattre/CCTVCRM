/**
 * AppProviders — composes all app-level React context providers into a
 * single wrapper component.
 *
 * Provider order (outer → inner):
 *   ThemeProvider        — active theme adapter (context only; no visual output)
 *   QueryClientProvider  — TanStack Query: caching, background refetch
 *   AuthProvider         — session restoration + silent token refresh
 *   ReactQueryDevtools   — visible in development only
 *
 * The interceptors are wired once here, not inside a React component lifecycle,
 * to guarantee they are registered before any request can fire.
 *
 * Usage in main.tsx:
 *   <AppProviders>
 *     <AppRouter />
 *   </AppProviders>
 */

import { type ReactNode, useRef } from 'react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { AuthProvider } from '@/core/auth/AuthProvider';
import { setupInterceptors } from '@/core/api/interceptors';
import { ToastContainer } from '@/shared/ui/toast';
import { ThemeProvider } from '@/theme';

// Register interceptors immediately when this module loads (once per app start).
setupInterceptors();

// ── QueryClient configuration ─────────────────────────────────────────────────

function makeQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      queries: {
        // Don't retry on 4xx errors (auth errors, not-found) — only on network failures.
        retry: (failureCount, error) => {
          const status = (error as { response?: { status?: number } })?.response?.status;
          if (status && status >= 400 && status < 500) return false;
          return failureCount < 2;
        },
        staleTime: 30_000, // Data stays fresh for 30 s before a background refetch.
        refetchOnWindowFocus: false,
      },
      mutations: {
        retry: false,
      },
    },
  });
}

// ── Provider tree ─────────────────────────────────────────────────────────────

interface AppProvidersProps {
  children: ReactNode;
}

export function AppProviders({ children }: AppProvidersProps) {
  // Use useRef so the QueryClient is stable across React Strict Mode double-mounts.
  const queryClientRef = useRef<QueryClient | null>(null);
  if (!queryClientRef.current) {
    queryClientRef.current = makeQueryClient();
  }

  return (
    <ThemeProvider>
      <QueryClientProvider client={queryClientRef.current}>
        <AuthProvider>
          {children}
          <ToastContainer />

          {/* DevTools panel — tree-shaken in production builds */}
          {import.meta.env.DEV && <ReactQueryDevtools initialIsOpen={false} />}
        </AuthProvider>
      </QueryClientProvider>
    </ThemeProvider>
  );
}

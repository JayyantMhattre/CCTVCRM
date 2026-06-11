/**
 * AuthProvider — bootstraps the authentication session on app load.
 *
 * What it does:
 *  1. On mount, tries to restore a valid session from sessionStorage.
 *  2. If a session exists but the token is near expiry, triggers a silent
 *     refresh so the user never sees an unexpected 401 mid-session.
 *  3. If no session is found, marks loading as false so the router can
 *     redirect unauthenticated users to /login.
 *
 * This component is purely behavioural — it renders nothing itself, only
 * wrapping its children. It must be placed INSIDE QueryClientProvider
 * and OUTSIDE the router so guards can read the hydrated store synchronously.
 */

import { type ReactNode, useEffect } from 'react';
import { useAuthStore, restoreSessionFromStorage } from './authStore';
import { tokenService } from './tokenService';

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const { setLoading, clearSession } = useAuthStore();

  useEffect(() => {
    async function bootstrap(): Promise<void> {
      // Step 1: try to restore session from storage.
      const restored = restoreSessionFromStorage();

      if (!restored) {
        // No valid persisted session — show the app in unauthenticated state.
        setLoading(false);
        return;
      }

      // Step 2: proactively refresh if the token expires in <5 minutes.
      const { expiresAt } = useAuthStore.getState();
      const fiveMinutes   = 5 * 60 * 1_000;

      if (expiresAt !== null && Date.now() > expiresAt - fiveMinutes) {
        try {
          await tokenService.refresh();
        } catch {
          // Refresh failed (e.g. refresh token expired) — force re-login.
          clearSession();
        }
      }

      setLoading(false);
    }

    void bootstrap();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // run once on mount

  // While loading, render nothing — the router will show a spinner via AuthGuard.
  return <>{children}</>;
}

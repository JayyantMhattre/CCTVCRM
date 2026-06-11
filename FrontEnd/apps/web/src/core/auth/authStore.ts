/**
 * Global authentication state — Zustand store.
 *
 * This is the single source of truth for the current user's session.
 * All guards, hooks, and the Axios interceptor read from here.
 *
 * Persistence strategy:
 * - `accessToken` is kept in memory only (most secure against XSS).
 * - On page reload the token is restored from `sessionStorage` so
 *   the user is not logged out when refreshing a tab.
 * - `sessionStorage` is cleared when the browser tab closes (not shared
 *   across tabs like `localStorage`). Use `localStorage` if you want
 *   persistent login across tabs/restarts.
 *
 * Session restoration happens in `AuthProvider.tsx` on app mount.
 */

import { create } from 'zustand';
import type { AuthUser } from '@/shared/types/auth.types';

// ── Store shape ─────────────────────────────────────────────────────────────

interface AuthState {
  /** The raw JWT access token (Bearer). Null when unauthenticated. */
  accessToken:  string | null;
  /** The refresh token used to silently obtain a new access token. */
  refreshToken: string | null;
  /** UTC timestamp (ms) when the access token expires. */
  expiresAt:    number | null;
  /** Decoded user data extracted from the JWT payload. */
  user:         AuthUser | null;
  /** True while the initial session restoration is in progress. */
  isLoading:    boolean;
}

interface AuthActions {
  /**
   * Called after a successful login or token refresh.
   * Sets all auth state in one atomic update.
   */
  setSession: (params: {
    accessToken:  string;
    refreshToken: string;
    expiresAt:    number;
    user:         AuthUser;
  }) => void;

  /**
   * Clears the entire session (logout, refresh failure, 401).
   * Also wipes sessionStorage to prevent stale data on next reload.
   */
  clearSession: () => void;

  /** Used by AuthProvider to hide the loading spinner once restored. */
  setLoading: (loading: boolean) => void;
}

// ── Session storage key ─────────────────────────────────────────────────────

const SESSION_KEY = 'ashraak_session';

/** Persisted shape — only non-sensitive identifiers, NOT the raw tokens. */
interface PersistedSession {
  accessToken:  string;
  refreshToken: string;
  expiresAt:    number;
  user:         AuthUser;
}

// ── Store implementation ─────────────────────────────────────────────────────

export const useAuthStore = create<AuthState & AuthActions>((set) => ({
  // Initial state
  accessToken:  null,
  refreshToken: null,
  expiresAt:    null,
  user:         null,
  isLoading:    true, // true until AuthProvider completes restoration

  setSession: ({ accessToken, refreshToken, expiresAt, user }) => {
    // Persist to sessionStorage for reload recovery.
    const session: PersistedSession = { accessToken, refreshToken, expiresAt, user };
    sessionStorage.setItem(SESSION_KEY, JSON.stringify(session));

    set({ accessToken, refreshToken, expiresAt, user, isLoading: false });
  },

  clearSession: () => {
    sessionStorage.removeItem(SESSION_KEY);
    set({
      accessToken:  null,
      refreshToken: null,
      expiresAt:    null,
      user:         null,
      isLoading:    false,
    });
  },

  setLoading: (loading) => set({ isLoading: loading }),
}));

// ── Helper: restore session from sessionStorage ─────────────────────────────

/**
 * Reads the persisted session and, if still valid, hydrates the store.
 * Called once by `AuthProvider` on mount.
 *
 * @returns `true` if a valid (non-expired) session was restored.
 */
export function restoreSessionFromStorage(): boolean {
  try {
    const raw = sessionStorage.getItem(SESSION_KEY);
    if (!raw) return false;

    const session = JSON.parse(raw) as PersistedSession;

    // Reject if the token is already expired (with 60-second buffer).
    if (Date.now() > session.expiresAt - 60_000) return false;

    useAuthStore.getState().setSession(session);
    return true;
  } catch {
    // Corrupt or tampered storage — ignore and treat as unauthenticated.
    sessionStorage.removeItem(SESSION_KEY);
    return false;
  }
}

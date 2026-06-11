/**
 * useAuth — primary hook for reading authentication state.
 *
 * Provides a clean, typed interface to the Zustand auth store.
 * All components should use this hook instead of importing the store directly.
 *
 * Returns:
 *   status         — 'loading' | 'authenticated' | 'unauthenticated'
 *   user           — AuthUser | null
 *   isAuthenticated — boolean shortcut
 *   isLoading      — boolean shortcut (use to show skeleton/spinner)
 *   hasRole(name)  — true if the user has the named role
 */

import { useAuthStore } from '@/core/auth/authStore';
import type { AuthStatus } from '@/shared/types/auth.types';

export function useAuth() {
  const { user, isLoading, accessToken, clearSession } = useAuthStore();

  const status: AuthStatus =
    isLoading            ? 'loading'
    : accessToken !== null ? 'authenticated'
    :                        'unauthenticated';

  /**
   * Returns true if the user has AT LEAST ONE of the provided roles.
   * Case-sensitive to match the backend role string exactly.
   */
  function hasRole(...roles: string[]): boolean {
    if (!user) return false;
    return roles.some((r) => user.roles.includes(r));
  }

  return {
    status,
    user,
    isAuthenticated: status === 'authenticated',
    isLoading:       status === 'loading',
    hasRole,
    /** Call to log the user out from any component (e.g., header button). */
    logout: clearSession,
  };
}

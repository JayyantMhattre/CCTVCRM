/**
 * Token service — decodes JWTs and manages the refresh flow.
 *
 * Responsibilities:
 *  1. Decode the raw JWT string into a typed `DecodedToken` object.
 *  2. Map the backend's custom claims (`tenant_id`, `permission`, `role`)
 *     to a structured `AuthUser` object that the rest of the app uses.
 *  3. Execute a silent token refresh when the current token is near expiry.
 *
 * The service does NOT store anything — state management is `authStore.ts`.
 */

import { jwtDecode }   from 'jwt-decode';
import { apiClient }   from '@/core/api/client';
import { ENDPOINTS }   from '@/core/api/endpoints';
import { useAuthStore } from './authStore';
import type { AuthUser, DecodedToken } from '@/shared/types/auth.types';

// ── JWT decoding ─────────────────────────────────────────────────────────────

/**
 * Decodes a raw JWT string and extracts all known claims.
 * Returns `null` if the token is missing or malformed.
 */
export function decodeToken(raw: string): DecodedToken | null {
  try {
    return jwtDecode<DecodedToken>(raw);
  } catch {
    return null;
  }
}

/**
 * Maps the decoded JWT claims to the `AuthUser` shape used by the app.
 *
 * Claim mapping (backend → frontend):
 *   sub         → userId
 *   email       → email
 *   name        → displayName
 *   tenant_id   → tenantId  (primary; also checked as "tenantId")
 *   role        → roles[]   (OpenIddict emits one claim per role)
 *   permission  → permissions[]
 */
export function mapTokenToUser(decoded: DecodedToken): AuthUser {
  // OpenIddict emits multi-value claims as an array or a single string.
  const toArray = (value: string | string[] | undefined): string[] => {
    if (!value)                return [];
    if (Array.isArray(value))  return value;
    return [value];
  };

  return {
    userId:      decoded.sub ?? '',
    email:       decoded.email ?? '',
    displayName: decoded.name ?? decoded.email ?? '',
    tenantId:    decoded.tenant_id ?? decoded.tenantId ?? '',
    roles:       toArray(decoded.role),
    permissions: toArray(decoded.permission),
  };
}

// ── Token refresh ─────────────────────────────────────────────────────────────

/**
 * Attempts to obtain a new access token using the stored refresh token.
 * On success, updates the auth store.
 * On failure, throws so the interceptor can clear the session.
 */
export async function refresh(): Promise<void> {
  const store = useAuthStore.getState();
  const refreshToken = store.refreshToken;

  if (!refreshToken) throw new Error('No refresh token available.');

  // OpenIddict refresh_token grant — form-encoded as per OAuth 2.0 spec.
  const params = new URLSearchParams({
    grant_type:    'refresh_token',
    refresh_token: refreshToken,
  });

  const response = await apiClient.post<{
    access_token:  string;
    refresh_token: string;
    expires_in:    number;
  }>(ENDPOINTS.auth.token, params.toString(), {
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
  });

  const { access_token, refresh_token, expires_in } = response.data;
  const decoded = decodeToken(access_token);
  if (!decoded) throw new Error('Received malformed access token during refresh.');

  store.setSession({
    accessToken:  access_token,
    refreshToken: refresh_token,
    expiresAt:    Date.now() + expires_in * 1_000,
    user:         mapTokenToUser(decoded),
  });
}

/** Exported object — used by interceptors.ts */
export const tokenService = { refresh };

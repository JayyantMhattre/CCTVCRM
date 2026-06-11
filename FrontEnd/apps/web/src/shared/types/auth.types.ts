/**
 * Authentication and authorisation type definitions.
 *
 * These types are backend-agnostic — the token service maps raw JWT claims
 * to these shapes so the rest of the app never deals with raw claim strings.
 */

// ── Decoded JWT payload ───────────────────────────────────────────────────────

/**
 * Raw claims from the JWT payload as emitted by the backend.
 * Use `AuthUser` (mapped below) for all application code.
 *
 * Multi-value claims (roles, permissions) may arrive as a single string
 * OR an array depending on how many values the token contains.
 */
export interface DecodedToken {
  /** Standard JWT subject — maps to userId. */
  sub:         string;
  /** User email address. */
  email:       string;
  /** Display name (optional). */
  name?:       string;
  /** Tenant identifier — primary claim name. */
  tenant_id?:  string;
  /** Tenant identifier — alternate claim name (backwards compat). */
  tenantId?:   string;
  /** Role claim — string OR string[] for multi-role tokens. */
  role?:       string | string[];
  /** Permission claim — string OR string[]. */
  permission?: string | string[];
  /** JWT expiry (Unix seconds). */
  exp?:        number;
  /** JWT issued-at (Unix seconds). */
  iat?:        number;
}

// ── Application user ──────────────────────────────────────────────────────────

/**
 * The structured user object used throughout the application.
 * Created by `tokenService.mapTokenToUser()` from `DecodedToken`.
 */
export interface AuthUser {
  userId:      string;
  email:       string;
  displayName: string;
  /** The tenant this user belongs to. Empty string when not tenant-scoped. */
  tenantId:    string;
  /** All roles assigned to this user for the current tenant. */
  roles:       string[];
  /** All fine-grained permissions granted to this user. */
  permissions: string[];
}

// ── Session state helpers ─────────────────────────────────────────────────────

/** Represents a fully authenticated session. */
export interface AuthSession {
  accessToken:  string;
  refreshToken: string;
  /** UTC timestamp (ms) when the access token expires. */
  expiresAt:    number;
  user:         AuthUser;
}

/** Possible auth states used by guards and hooks. */
export type AuthStatus = 'loading' | 'authenticated' | 'unauthenticated';

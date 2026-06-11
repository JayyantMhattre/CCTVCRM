/**
 * Auth module type definitions.
 *
 * These types describe request/response shapes specific to the auth module.
 * They are NOT exported outside the module — use `AuthUser` from shared/types
 * for cross-module auth data.
 */

// ── Login ─────────────────────────────────────────────────────────────────────

export interface LoginRequest {
  /** User's email address (maps to OAuth `username` field). */
  email:    string;
  password: string;
  /** The tenant the user is logging into. */
  tenantId: string;
}

/** Raw OAuth 2.0 token response from the backend. */
export interface TokenResponse {
  access_token:  string;
  refresh_token: string;
  token_type:    string;
  /** Lifetime in seconds. */
  expires_in:    number;
}

// ── Registration ──────────────────────────────────────────────────────────────

export interface RegisterRequest {
  tenantId:    string;
  email:       string;
  password:    string;
  displayName: string;
}

export interface RegisterResponse {
  userId: string;
}

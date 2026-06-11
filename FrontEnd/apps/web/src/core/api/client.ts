/**
 * Shared Axios HTTP client.
 *
 * All modules import `apiClient` from here — never create their own instance.
 * The request/response interceptors (JWT injection, 401 retry) are wired
 * in `interceptors.ts` which is imported once from `AppProviders.tsx`.
 *
 * Design decisions:
 * - `baseURL` comes from `VITE_API_BASE_URL` so it's environment-driven.
 * - In development, Vite's proxy forwards `/api/*` and `/connect/*` to the
 *   backend, so `baseURL` can be left as `/` during dev and a real domain
 *   in production without code changes.
 * - `Content-Type` defaults to `application/json`; the Auth module
 *   overrides it to `application/x-www-form-urlencoded` for the token endpoint.
 */

import axios from 'axios';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '',
  timeout: 15_000,
  headers: {
    'Content-Type': 'application/json',
    Accept:         'application/json',
  },
  withCredentials: true, // send cookies (refresh_token httpOnly cookie if server uses it)
});

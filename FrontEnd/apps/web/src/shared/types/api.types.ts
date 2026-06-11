/**
 * Generic API response shapes.
 *
 * These types are reused by every module's API layer and TanStack Query hooks.
 * They mirror the backend's ProblemDetails and paginated list conventions.
 */

// ── Success shapes ────────────────────────────────────────────────────────────

/** A standard paginated list response. */
export interface PaginatedResponse<T> {
  items:       T[];
  page:        number;
  pageSize:    number;
  totalCount:  number;
  totalPages:  number;
}

/** A minimal created-resource response from a POST endpoint. */
export interface CreatedResponse {
  /** The newly created resource identifier. */
  id: string;
}

// ── Error shapes ──────────────────────────────────────────────────────────────

/**
 * RFC 7807 ProblemDetails — the error format returned by the backend.
 * https://datatracker.ietf.org/doc/html/rfc7807
 */
export interface ProblemDetails {
  type?:     string;
  title?:    string;
  status?:   number;
  detail?:   string;
  instance?: string;
  /** Extension fields (e.g. traceId, errors). */
  [key: string]: unknown;
}

// ── Common value types ────────────────────────────────────────────────────────

/** UUID string — used for IDs to make intent explicit in function signatures. */
export type Uuid = string;

/** ISO 8601 datetime string as returned by the API. */
export type IsoDate = string;

/** A generic label/value pair for dropdowns. */
export interface SelectOption<T = string> {
  label: string;
  value: T;
}

/**
 * Correlation ID helpers — aligned with backend X-Correlation-Id middleware.
 */

export const CORRELATION_HEADER = 'X-Correlation-Id';

let lastCorrelationId: string | null = null;

export function createCorrelationId(): string {
  return crypto.randomUUID().replace(/-/g, '');
}

export function getLastCorrelationId(): string | null {
  return lastCorrelationId;
}

export function setLastCorrelationId(id: string | null): void {
  lastCorrelationId = id;
}

export function readCorrelationIdFromHeaders(
  headers: Record<string, unknown> | undefined,
): string | null {
  if (!headers) return null;
  const value =
    headers[CORRELATION_HEADER] ??
    headers[CORRELATION_HEADER.toLowerCase()] ??
    headers['x-correlation-id'];
  if (typeof value === 'string' && value.length > 0) return value;
  if (Array.isArray(value) && typeof value[0] === 'string') return value[0];
  return null;
}

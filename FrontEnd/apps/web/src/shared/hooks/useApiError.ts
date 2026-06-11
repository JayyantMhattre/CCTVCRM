/**
 * useApiError — normalises an Axios error or unknown thrown value
 * into a human-readable string for display in UI components.
 *
 * Priority:
 *  1. Backend ProblemDetails `detail` field.
 *  2. Backend ProblemDetails `title` field.
 *  3. Axios network error message.
 *  4. Generic fallback.
 *
 * Example:
 *   const { extractMessage } = useApiError();
 *   toast.error(extractMessage(mutationError));
 */

import { classifyApiError } from '@/shared/errors/apiErrorClassifier';
import { getLastCorrelationId } from '@/shared/errors/correlationId';

export function useApiError() {
  function extractMessage(error: unknown): string {
    if (!error) return 'An unknown error occurred.';
    return classifyApiError(error).message;
  }

  function extractCorrelationId(error: unknown): string | null {
    const classified = classifyApiError(error);
    return classified.correlationId ?? getLastCorrelationId();
  }

  return { extractMessage, extractCorrelationId, classify: classifyApiError };
}

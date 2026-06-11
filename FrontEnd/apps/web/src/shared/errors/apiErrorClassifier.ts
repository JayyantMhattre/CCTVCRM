/**
 * Maps Axios / network errors to user-friendly messages and toast variants.
 */

import type { AxiosError } from 'axios';
import type { ProblemDetails } from '@/shared/types/api.types';
import type { ToastVariant } from '@/shared/ui/toast/toast.types';
import { readCorrelationIdFromHeaders } from './correlationId';

export type ApiErrorCategory =
  | 'validation'
  | 'auth'
  | 'forbidden'
  | 'notFound'
  | 'rateLimit'
  | 'server'
  | 'network'
  | 'unknown';

export interface ClassifiedApiError {
  category: ApiErrorCategory;
  variant: ToastVariant;
  title: string;
  message: string;
  correlationId: string | null;
  status?: number;
}

function problemDetail(error: AxiosError<ProblemDetails>): string | null {
  const data = error.response?.data;
  if (!data) return null;
  if (typeof data.detail === 'string' && data.detail.length > 0) return data.detail;
  if (typeof data.title === 'string' && data.title.length > 0) return data.title;
  return null;
}

export function classifyApiError(error: unknown): ClassifiedApiError {
  const axiosError = error as AxiosError<ProblemDetails>;
  const status = axiosError.response?.status;
  const correlationId = readCorrelationIdFromHeaders(
    axiosError.response?.headers as Record<string, unknown> | undefined,
  );
  const detail = problemDetail(axiosError);

  if (!axiosError.response) {
    return {
      category: 'network',
      variant: 'warning',
      title: 'Connection problem',
      message: 'Unable to reach the server. Check your network and try again.',
      correlationId,
    };
  }

  if (status === 400 || status === 422) {
    return {
      category: 'validation',
      variant: 'warning',
      title: 'Invalid request',
      message: detail ?? 'Please check your input and try again.',
      correlationId,
      status,
    };
  }

  if (status === 401) {
    return {
      category: 'auth',
      variant: 'warning',
      title: 'Session expired',
      message: detail ?? 'Please sign in again.',
      correlationId,
      status,
    };
  }

  if (status === 403) {
    return {
      category: 'forbidden',
      variant: 'warning',
      title: 'Access denied',
      message: detail ?? 'You do not have permission to perform this action.',
      correlationId,
      status,
    };
  }

  if (status === 404) {
    return {
      category: 'notFound',
      variant: 'info',
      title: 'Not found',
      message: detail ?? 'The requested resource could not be found.',
      correlationId,
      status,
    };
  }

  if (status === 429) {
    const retryAfter = axiosError.response?.headers['retry-after'];
    const retryHint =
      typeof retryAfter === 'string' && retryAfter.length > 0
        ? ` Try again in ${retryAfter} seconds.`
        : '';
    return {
      category: 'rateLimit',
      variant: 'warning',
      title: 'Too many requests',
      message: (detail ?? 'You have sent too many requests.') + retryHint,
      correlationId,
      status,
    };
  }

  if (status && status >= 500) {
    return {
      category: 'server',
      variant: 'error',
      title: 'Server error',
      message: detail ?? 'Something went wrong on our side. Please try again later.',
      correlationId,
      status,
    };
  }

  return {
    category: 'unknown',
    variant: 'error',
    title: 'Request failed',
    message: detail ?? axiosError.message ?? 'An unexpected error occurred.',
    correlationId,
    status,
  };
}

/** Whether the global interceptor should show a toast for this error. */
export function shouldToastApiError(classified: ClassifiedApiError): boolean {
  // 401 is handled by silent refresh / redirect — avoid duplicate toasts.
  return classified.category !== 'auth';
}

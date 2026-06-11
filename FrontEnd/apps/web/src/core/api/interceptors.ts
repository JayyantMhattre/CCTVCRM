/**
 * Axios request / response interceptors.
 *
 * Call `setupInterceptors()` ONCE from `AppProviders.tsx` during app bootstrap.
 */

import type { AxiosError, InternalAxiosRequestConfig } from 'axios';
import { apiClient } from './client';
import { useAuthStore } from '@/core/auth/authStore';
import { tokenService } from '@/core/auth/tokenService';
import {
  CORRELATION_HEADER,
  createCorrelationId,
  setLastCorrelationId,
  readCorrelationIdFromHeaders,
} from '@/shared/errors/correlationId';
import {
  classifyApiError,
  shouldToastApiError,
} from '@/shared/errors/apiErrorClassifier';
import { toastService } from '@/shared/ui/toast';

interface RetryConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
  /** Skip global error toast (e.g. expected 404 on optional settings GET). */
  _skipErrorToast?: boolean;
}

declare module 'axios' {
  export interface AxiosRequestConfig {
    _skipErrorToast?: boolean;
  }
}

export function setupInterceptors(): void {
  apiClient.interceptors.request.use(
    (config) => {
      const token = useAuthStore.getState().accessToken;
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }

      const existing = config.headers[CORRELATION_HEADER];
      const correlationId =
        (typeof existing === 'string' && existing.length > 0
          ? existing
          : createCorrelationId());
      config.headers[CORRELATION_HEADER] = correlationId;
      setLastCorrelationId(correlationId);

      return config;
    },
    (error: unknown) => Promise.reject(error),
  );

  apiClient.interceptors.response.use(
    (response) => {
      const fromResponse = readCorrelationIdFromHeaders(
        response.headers as Record<string, unknown>,
      );
      if (fromResponse) setLastCorrelationId(fromResponse);
      return response;
    },

    async (error: AxiosError) => {
      const originalRequest = error.config as RetryConfig | undefined;

      const fromError = readCorrelationIdFromHeaders(
        error.response?.headers as Record<string, unknown> | undefined,
      );
      if (fromError) setLastCorrelationId(fromError);

      if (
        error.response?.status === 401 &&
        originalRequest &&
        !originalRequest._retry
      ) {
        originalRequest._retry = true;

        try {
          await tokenService.refresh();
          const newToken = useAuthStore.getState().accessToken;
          if (newToken) {
            originalRequest.headers.Authorization = `Bearer ${newToken}`;
          }
          return apiClient(originalRequest);
        } catch {
          useAuthStore.getState().clearSession();
          window.location.href = '/login';
          return Promise.reject(error);
        }
      }

      if (!originalRequest?._skipErrorToast) {
        const classified = classifyApiError(error);
        if (shouldToastApiError(classified)) {
          toastService[classified.variant](classified.message, {
            title: classified.title,
            correlationId: classified.correlationId ?? undefined,
            autoDismissMs: classified.category === 'rateLimit' ? 8_000 : 5_000,
          });
        }
      }

      return Promise.reject(error);
    },
  );
}

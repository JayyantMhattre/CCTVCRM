/**
 * Auth module API functions.
 */

import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type { LoginRequest, RegisterRequest, RegisterResponse, TokenResponse } from './types';

export interface SessionDto {
  id: string;
  createdOnUtc: string;
  lastUsedOnUtc: string;
  ipAddress: string;
  userAgent: string;
  isRevoked: boolean;
}

const AUTH = `${import.meta.env.VITE_API_VERSION ? `/api/${import.meta.env.VITE_API_VERSION}` : '/api/v1'}/auth`;

export const authApi = {
  login: async (data: LoginRequest): Promise<TokenResponse> => {
    const params = new URLSearchParams({
      grant_type: 'password',
      username: data.email,
      password: data.password,
      tenant_id: data.tenantId,
    });

    const response = await apiClient.post<TokenResponse>(
      ENDPOINTS.auth.token,
      params.toString(),
      { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } },
    );

    return response.data;
  },

  register: async (data: RegisterRequest): Promise<RegisterResponse> => {
    const response = await apiClient.post<RegisterResponse>(ENDPOINTS.auth.register, data);
    return response.data;
  },

  listSessions: async (): Promise<SessionDto[]> => {
    const res = await apiClient.get<SessionDto[]>(`${AUTH}/sessions`);
    return res.data;
  },

  revokeSession: async (sessionId: string): Promise<void> => {
    await apiClient.post(`${AUTH}/sessions/${sessionId}/revoke`);
  },

  revokeAllSessions: async (): Promise<void> => {
    await apiClient.post(`${AUTH}/sessions/revoke-all`);
  },
};

import { apiClient } from '@/core/api/client';

const AUTH = `${import.meta.env.VITE_API_VERSION ? `/api/${import.meta.env.VITE_API_VERSION}` : '/api/v1'}/auth`;

export interface PasswordResetChallenge {
  challengeId: string;
}

export const passwordResetApi = {
  requestOtp: async (tenantId: string, email: string, phoneNumber?: string): Promise<void> => {
    await apiClient.post(`${AUTH}/password-reset/request`, {
      tenantId,
      email,
      phoneNumber: phoneNumber ?? null,
    });
  },

  verifyOtp: async (tenantId: string, email: string, otpCode: string): Promise<PasswordResetChallenge> => {
    const res = await apiClient.post<PasswordResetChallenge>(`${AUTH}/password-reset/verify`, {
      tenantId,
      email,
      otpCode,
    });
    return res.data;
  },

  confirm: async (challengeId: string, newPassword: string): Promise<void> => {
    await apiClient.post(`${AUTH}/password-reset/confirm`, { challengeId, newPassword });
  },
};

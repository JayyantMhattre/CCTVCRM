/**
 * Users module API functions.
 */

import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type { UserDto, UserPreferencesDto } from './types';

export const usersApi = {
  listForCurrentTenant: async (): Promise<UserDto[]> => {
    const res = await apiClient.get<UserDto[]>(ENDPOINTS.users.currentTenant);
    return res.data;
  },

  listByTenant: async (tenantId: string): Promise<UserDto[]> => {
    const res = await apiClient.get<UserDto[]>(ENDPOINTS.users.byTenant(tenantId));
    return res.data;
  },

  getById: async (userId: string): Promise<UserDto> => {
    const res = await apiClient.get<UserDto>(ENDPOINTS.users.byId(userId));
    return res.data;
  },

  updatePreferences: async (
    userId: string,
    preferences: Partial<UserPreferencesDto>,
  ): Promise<UserPreferencesDto> => {
    const res = await apiClient.patch<UserPreferencesDto>(
      ENDPOINTS.users.preferences(userId),
      preferences,
    );
    return res.data;
  },
};

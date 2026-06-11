/**
 * Tenant module API functions.
 */

import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';
import type { TenantDto, TenantSettingsDto } from './types';

export const DEFAULT_TENANT_SETTINGS: TenantSettingsDto = {
  locale:            'en-US',
  timezone:          'UTC',
  passwordMinLength: 8,
  requireMfa:        false,
  sessionTimeoutMinutes: 480,
};

export const tenantApi = {
  getCurrent: async (): Promise<TenantDto> => {
    const res = await apiClient.get<TenantDto>(ENDPOINTS.tenants.current);
    return res.data;
  },

  getById: async (tenantId: string): Promise<TenantDto> => {
    const res = await apiClient.get<TenantDto>(ENDPOINTS.tenants.byId(tenantId));
    return res.data;
  },

  getSettings: async (): Promise<TenantSettingsDto> => {
    const res = await apiClient.get<TenantSettingsDto>(ENDPOINTS.tenants.settings);
    return res.data;
  },

  /** PATCH tenant settings — Admin/Manager workspace configuration. */
  updateSettings: async (settings: TenantSettingsDto): Promise<TenantSettingsDto> => {
    const res = await apiClient.patch<TenantSettingsDto>(
      ENDPOINTS.tenants.settings,
      settings,
    );
    return res.data;
  },
};

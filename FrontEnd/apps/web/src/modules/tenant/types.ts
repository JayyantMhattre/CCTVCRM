/**
 * Tenant module type definitions.
 * These mirror the DTOs returned by the backend's Tenant module.
 */

import type { IsoDate } from '@/shared/types/api.types';

export type TenantPlan = 'Free' | 'Starter' | 'Pro' | 'Enterprise';
export type TenantStatus = 'Active' | 'Suspended' | 'Deleted';

export interface TenantDto {
  tenantId:     string;
  name:         string;
  slug:         string;
  plan:         TenantPlan;
  status:       TenantStatus;
  customDomain: string | null;
  createdOnUtc: IsoDate;
}

export interface TenantSettingsDto {
  locale:            string;
  timezone:          string;
  passwordMinLength: number;
  requireMfa:        boolean;
  sessionTimeoutMinutes: number;
}

export interface ProvisionTenantRequest {
  name:        string;
  slug:        string;
  plan:        TenantPlan;
  ownerUserId: string;
}

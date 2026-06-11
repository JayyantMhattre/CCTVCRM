/**
 * Users module type definitions.
 * Mirror the UserDto and UserPreferencesDto from the backend.
 */

import type { IsoDate } from '@/shared/types/api.types';

export type UserStatus = 'Active' | 'Inactive' | 'Suspended';

export interface UserPreferencesDto {
  theme:                    string;
  locale:                   string;
  timezone:                 string;
  emailNotificationsEnabled: boolean;
}

export interface UserDto {
  userId:      string;
  tenantId:    string;
  email:       string;
  displayName: string;
  avatarUrl:   string | null;
  preferences: UserPreferencesDto;
  status:      UserStatus;
  createdOnUtc: IsoDate;
}

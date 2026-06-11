import { usePermission } from '@/shared/hooks/usePermission';
import { API_KEY_PERMISSIONS } from '../types';

export function useApiKeyPermissions() {
  const { hasPermission } = usePermission();

  const canRead =
    hasPermission(API_KEY_PERMISSIONS.read) ||
    hasPermission(API_KEY_PERMISSIONS.manage);

  const canManage = hasPermission(API_KEY_PERMISSIONS.manage);

  return { canRead, canManage };
}

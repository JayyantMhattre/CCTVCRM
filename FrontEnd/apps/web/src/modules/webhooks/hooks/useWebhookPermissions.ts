import { usePermission } from '@/shared/hooks/usePermission';
import { WEBHOOK_PERMISSIONS } from '../types';

export function useWebhookPermissions() {
  const { hasPermission } = usePermission();

  const canRead =
    hasPermission(WEBHOOK_PERMISSIONS.read) ||
    hasPermission(WEBHOOK_PERMISSIONS.manage);

  const canManage = hasPermission(WEBHOOK_PERMISSIONS.manage);

  return { canRead, canManage };
}

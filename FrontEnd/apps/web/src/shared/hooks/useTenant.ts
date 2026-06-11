/**
 * useTenant — reads the current tenant context from the JWT.
 *
 * The tenantId is decoded from the access token by the token service and
 * stored in the auth store's `user.tenantId` field.
 *
 * Example:
 *   const { tenantId } = useTenant();
 *   // Use in API calls that require an explicit tenantId parameter.
 */

import { useAuthStore } from '@/core/auth/authStore';

export function useTenant() {
  const user = useAuthStore((s) => s.user);

  return {
    /** The tenant UUID extracted from the JWT, or null if not authenticated. */
    tenantId: user?.tenantId ?? null,
    /** True if a tenant context is available (user is authenticated). */
    hasTenant: Boolean(user?.tenantId),
  };
}

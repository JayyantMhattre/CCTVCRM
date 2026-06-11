/**
 * usePlatformNotify — façade over the platform toast service.
 *
 * Gives modules a stable, theme-agnostic notification API. It simply forwards
 * to the existing `useToast` hook so there is one notification implementation;
 * modules depend on `platform-ui` rather than on the toast internals.
 */

import { useToast } from '@/shared/ui/toast';

export function usePlatformNotify() {
  const { success, warning, error, info, show, dismiss } = useToast();
  return { success, warning, error, info, show, dismiss };
}

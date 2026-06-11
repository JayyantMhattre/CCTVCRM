/**
 * CoreUI adapter — notification viewport.
 *
 * Wraps the existing platform `ToastContainer`. The toast queue/service is
 * platform-owned and theme-agnostic; only this viewport is themed.
 */

import { ToastContainer } from '@/shared/ui/toast';

export function CoreUiNotificationViewport() {
  return <ToastContainer />;
}

/**
 * HexaDash adapter — notification viewport.
 *
 * Wraps the platform-owned `ToastContainer`. The toast queue/service is
 * platform-owned and theme-agnostic; the theme only presents the stack. The
 * `.hexadash-theme` wrapper scopes HexaDash tokens onto the toasts.
 */

import { ToastContainer } from '@/shared/ui/toast';

export function HexaDashNotificationViewport() {
  return (
    <div className="hexadash-theme">
      <ToastContainer />
    </div>
  );
}

/**
 * NotificationContract — the notification (toast) viewport a theme implements.
 *
 * The notification *state and service* (queue, dismiss, autoclose) live in the
 * platform (`shared/ui/toast`) and are theme-agnostic. Only the *viewport* —
 * how the stack of toasts is visually presented — is themed.
 */

import type { ComponentType } from 'react';

/**
 * Props for the notification viewport. The viewport reads toast state from the
 * global store, so no props are required today; the shape is reserved for
 * future positioning/configuration options.
 */
export type PlatformNotificationViewportProps = Record<string, never>;

/** A theme's notification viewport component. */
export type PlatformNotificationViewport =
  ComponentType<PlatformNotificationViewportProps>;

/** The notification portion of a {@link ThemeAdapter}. */
export interface NotificationContract {
  Viewport: PlatformNotificationViewport;
}

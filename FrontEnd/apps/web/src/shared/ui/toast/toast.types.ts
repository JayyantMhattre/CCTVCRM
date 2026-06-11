/**
 * Global toast notification types.
 */

export type ToastVariant = 'success' | 'warning' | 'error' | 'info';

export interface ToastMessage {
  id: string;
  variant: ToastVariant;
  title?: string;
  message: string;
  /** Shown on API failures when the backend returns X-Correlation-Id. */
  correlationId?: string;
  /** Auto-dismiss delay in ms. Set 0 to require manual dismiss. Default 5000. */
  autoDismissMs?: number;
}

export interface ShowToastInput {
  variant: ToastVariant;
  title?: string;
  message: string;
  correlationId?: string;
  autoDismissMs?: number;
}

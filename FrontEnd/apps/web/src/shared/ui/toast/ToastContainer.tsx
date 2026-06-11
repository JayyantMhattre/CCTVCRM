/**
 * Fixed toast stack — CoreUI / Bootstrap toast styling.
 */

import { CIcon } from '@coreui/icons-react';
import {
  cilCheckCircle,
  cilWarning,
  cilInfo,
  cilXCircle,
} from '@coreui/icons';
import { CorrelationIdCopy } from '@/shared/components/CorrelationIdCopy';
import { useToast } from './useToast';
import type { ToastVariant } from './toast.types';

const variantIcon: Record<ToastVariant, string[]> = {
  success: cilCheckCircle,
  warning: cilWarning,
  error:   cilXCircle,
  info:    cilInfo,
};

const variantBg: Record<ToastVariant, string> = {
  success: 'text-bg-success',
  warning: 'text-bg-warning',
  error:   'text-bg-danger',
  info:    'text-bg-info',
};

export function ToastContainer() {
  const { toasts, dismiss } = useToast();

  if (toasts.length === 0) return null;

  return (
    <div
      className="toast-container position-fixed top-0 end-0 p-3"
      style={{ zIndex: 1090, maxWidth: '420px' }}
      aria-live="polite"
      aria-atomic="true"
    >
      {toasts.map((toast) => (
        <div
          key={toast.id}
          className={`toast show mb-2 border-0 shadow ${variantBg[toast.variant]}`}
          role="alert"
        >
          <div className="toast-header border-0 text-white bg-transparent">
            <CIcon
              icon={variantIcon[toast.variant]}
              className="me-2 flex-shrink-0"
              aria-hidden="true"
            />
            <strong className="me-auto">
              {toast.title ?? toast.variant.charAt(0).toUpperCase() + toast.variant.slice(1)}
            </strong>
            <button
              type="button"
              className="btn-close btn-close-white"
              aria-label="Dismiss"
              onClick={() => dismiss(toast.id)}
            />
          </div>
          <div className="toast-body text-white pt-0">
            <div>{toast.message}</div>
            {toast.correlationId && (
              <CorrelationIdCopy
                correlationId={toast.correlationId}
                className="mt-2 text-white-50"
              />
            )}
          </div>
        </div>
      ))}
    </div>
  );
}

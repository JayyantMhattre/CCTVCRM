/**
 * React hook for the global toast service.
 */

import { useToastStore } from './toastStore';
import { toastService } from './toastStore';
import type { ShowToastInput } from './toast.types';

export function useToast() {
  const toasts = useToastStore((s) => s.toasts);
  const dismiss = useToastStore((s) => s.dismiss);

  return {
    toasts,
    dismiss,
    show: (input: ShowToastInput) => useToastStore.getState().show(input),
    success: toastService.success,
    warning: toastService.warning,
    error: toastService.error,
    info: toastService.info,
  };
}

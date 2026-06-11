/**
 * Toast queue — Zustand store (usable outside React, e.g. Axios interceptors).
 */

import { create } from 'zustand';
import type { ShowToastInput, ToastMessage } from './toast.types';

const MAX_TOASTS = 5;
const DEFAULT_DISMISS_MS = 5_000;

interface ToastState {
  toasts: ToastMessage[];
  show: (input: ShowToastInput) => string;
  dismiss: (id: string) => void;
  clear: () => void;
}

function nextId(): string {
  return `toast-${Date.now()}-${Math.random().toString(36).slice(2, 9)}`;
}

export const useToastStore = create<ToastState>((set, get) => ({
  toasts: [],

  show: (input) => {
    const id = nextId();
    const toast: ToastMessage = {
      id,
      variant: input.variant,
      title: input.title,
      message: input.message,
      correlationId: input.correlationId,
      autoDismissMs: input.autoDismissMs ?? DEFAULT_DISMISS_MS,
    };

    set((state) => ({
      toasts: [...state.toasts, toast].slice(-MAX_TOASTS),
    }));

    const dismissMs = toast.autoDismissMs ?? 0;
    if (dismissMs > 0) {
      window.setTimeout(() => {
        if (get().toasts.some((t) => t.id === id)) {
          get().dismiss(id);
        }
      }, dismissMs);
    }

    return id;
  },

  dismiss: (id) =>
    set((state) => ({
      toasts: state.toasts.filter((t) => t.id !== id),
    })),

  clear: () => set({ toasts: [] }),
}));

/** Imperative API for interceptors and non-React code. */
export const toastService = {
  success: (message: string, options?: Omit<ShowToastInput, 'variant' | 'message'>) =>
    useToastStore.getState().show({ variant: 'success', message, ...options }),
  warning: (message: string, options?: Omit<ShowToastInput, 'variant' | 'message'>) =>
    useToastStore.getState().show({ variant: 'warning', message, ...options }),
  error: (message: string, options?: Omit<ShowToastInput, 'variant' | 'message'>) =>
    useToastStore.getState().show({ variant: 'error', message, ...options }),
  info: (message: string, options?: Omit<ShowToastInput, 'variant' | 'message'>) =>
    useToastStore.getState().show({ variant: 'info', message, ...options }),
  dismiss: (id: string) => useToastStore.getState().dismiss(id),
};

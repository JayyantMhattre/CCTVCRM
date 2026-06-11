/**
 * DialogContract — the modal/dialog surface a theme must implement.
 */

import type { ComponentType, ReactNode } from 'react';

/** Supported dialog sizes (maps to Bootstrap/CoreUI modal sizes). */
export type PlatformDialogSize = 'sm' | 'lg' | 'xl';

/** Props for a themed dialog. */
export interface PlatformDialogProps {
  /** Whether the dialog is visible. */
  show: boolean;
  /** Called when the dialog requests to close (backdrop, escape, close icon). */
  onClose: () => void;
  /** Optional header title. */
  title?: ReactNode;
  /** Optional footer content (e.g. action buttons). */
  footer?: ReactNode;
  /** Dialog size. */
  size?: PlatformDialogSize;
  /** Whether the dialog is vertically centred. Defaults to true. */
  centered?: boolean;
  /** Dialog body content. */
  children?: ReactNode;
}

/** A theme's dialog component. */
export type PlatformDialogComponent = ComponentType<PlatformDialogProps>;

/** The dialog portion of a {@link ThemeAdapter}. */
export interface DialogContract {
  Dialog: PlatformDialogComponent;
}

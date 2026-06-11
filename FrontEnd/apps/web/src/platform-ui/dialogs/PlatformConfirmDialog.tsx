/**
 * PlatformConfirmDialog — confirm/cancel dialog built on `PlatformDialog`.
 *
 * Theme-agnostic. Mirrors the behaviour of the existing module `ConfirmDialog`
 * so those usages can migrate to platform-ui in a later phase without change.
 */

import Button from 'react-bootstrap/Button';
import { PlatformDialog } from './PlatformDialog';

interface PlatformConfirmDialogProps {
  show: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  confirmVariant?: string;
  isLoading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export function PlatformConfirmDialog({
  show,
  title,
  message,
  confirmLabel = 'Confirm',
  cancelLabel = 'Cancel',
  confirmVariant = 'primary',
  isLoading = false,
  onConfirm,
  onCancel,
}: PlatformConfirmDialogProps) {
  return (
    <PlatformDialog
      show={show}
      title={title}
      onClose={onCancel}
      footer={
        <>
          <Button variant="outline-secondary" onClick={onCancel} disabled={isLoading}>
            {cancelLabel}
          </Button>
          <Button variant={confirmVariant} onClick={onConfirm} disabled={isLoading}>
            {isLoading ? 'Working…' : confirmLabel}
          </Button>
        </>
      }
    >
      {message}
    </PlatformDialog>
  );
}

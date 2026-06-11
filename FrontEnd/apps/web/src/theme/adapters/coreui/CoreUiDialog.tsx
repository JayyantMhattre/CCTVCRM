/**
 * CoreUI adapter — dialog.
 *
 * Built on `react-bootstrap/Modal` (CoreUI-compatible Bootstrap 5 markup),
 * mirroring the pattern already used by module dialogs such as `ConfirmDialog`.
 */

import Modal from 'react-bootstrap/Modal';
import type { PlatformDialogProps } from '@/theme/contracts';

export function CoreUiDialog({
  show,
  onClose,
  title,
  footer,
  size,
  centered = true,
  children,
}: PlatformDialogProps) {
  return (
    <Modal show={show} onHide={onClose} centered={centered} size={size}>
      {title !== undefined ? (
        <Modal.Header closeButton>
          <Modal.Title>{title}</Modal.Title>
        </Modal.Header>
      ) : null}

      <Modal.Body>{children}</Modal.Body>

      {footer !== undefined ? <Modal.Footer>{footer}</Modal.Footer> : null}
    </Modal>
  );
}

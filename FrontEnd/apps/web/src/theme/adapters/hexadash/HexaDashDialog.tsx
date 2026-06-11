/**
 * HexaDash adapter — dialog.
 *
 * Built on `react-bootstrap/Modal` (no Ant Design `Modal`), tagged with a
 * `.hexadash-theme` dialog class so the scoped HexaDash tokens apply to the
 * modal content, which `react-bootstrap` portals to `document.body`.
 */

import Modal from 'react-bootstrap/Modal';
import type { PlatformDialogProps } from '@/theme/contracts';

export function HexaDashDialog({
  show,
  onClose,
  title,
  footer,
  size,
  centered = true,
  children,
}: PlatformDialogProps) {
  return (
    <Modal
      show={show}
      onHide={onClose}
      centered={centered}
      size={size}
      dialogClassName="hexadash-theme hexadash-modal"
    >
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

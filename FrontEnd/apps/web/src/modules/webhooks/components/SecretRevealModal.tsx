import { useState } from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

interface SecretRevealModalProps {
  show: boolean;
  secret: string;
  title?: string;
  onClose: () => void;
}

export function SecretRevealModal({
  show,
  secret,
  title = 'Signing secret',
  onClose,
}: SecretRevealModalProps) {
  const [copied, setCopied] = useState(false);

  async function copySecret() {
    await navigator.clipboard.writeText(secret);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  return (
    <Modal show={show} onHide={onClose} centered>
      <Modal.Header closeButton>
        <Modal.Title>{title}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p className="small text-muted">
          Copy this secret now. It will not be shown again after you close this dialog.
        </p>
        <pre className="bg-light p-3 rounded small mb-0 text-break">{secret}</pre>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="outline-primary" onClick={() => void copySecret()}>
          {copied ? 'Copied' : 'Copy secret'}
        </Button>
        <Button variant="primary" onClick={onClose}>
          Done
        </Button>
      </Modal.Footer>
    </Modal>
  );
}

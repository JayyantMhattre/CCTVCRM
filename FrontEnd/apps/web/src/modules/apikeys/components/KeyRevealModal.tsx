import { useState } from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

interface KeyRevealModalProps {
  show: boolean;
  apiKey: string;
  title?: string;
  onClose: () => void;
}

export function KeyRevealModal({
  show,
  apiKey,
  title = 'API key',
  onClose,
}: KeyRevealModalProps) {
  const [copied, setCopied] = useState(false);

  async function copyKey() {
    await navigator.clipboard.writeText(apiKey);
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
          Copy this API key now. It will not be shown again after you close this dialog.
        </p>
        <pre className="bg-light p-3 rounded small mb-0 text-break">{apiKey}</pre>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="outline-primary" onClick={() => void copyKey()}>
          {copied ? 'Copied' : 'Copy key'}
        </Button>
        <Button variant="primary" onClick={onClose}>
          Done
        </Button>
      </Modal.Footer>
    </Modal>
  );
}

/**
 * Displays a correlation ID with a one-click copy action (support workflow).
 */

import { useState } from 'react';
import { CIcon } from '@coreui/icons-react';
import { cilCopy, cilCheckAlt } from '@coreui/icons';

interface CorrelationIdCopyProps {
  correlationId: string;
  className?: string;
}

export function CorrelationIdCopy({ correlationId, className = '' }: CorrelationIdCopyProps) {
  const [copied, setCopied] = useState(false);

  async function handleCopy() {
    try {
      await navigator.clipboard.writeText(correlationId);
      setCopied(true);
      window.setTimeout(() => setCopied(false), 2_000);
    } catch {
      /* clipboard denied — user can still select manually */
    }
  }

  return (
    <div className={`d-flex align-items-center gap-2 small ${className}`}>
      <span className="text-muted">Correlation ID:</span>
      <code className="font-monospace user-select-all">{correlationId}</code>
      <button
        type="button"
        className="btn btn-sm btn-outline-secondary py-0 px-2"
        onClick={() => void handleCopy()}
        aria-label="Copy correlation ID"
        title="Copy for support"
      >
        <CIcon icon={copied ? cilCheckAlt : cilCopy} size="sm" aria-hidden="true" />
      </button>
    </div>
  );
}

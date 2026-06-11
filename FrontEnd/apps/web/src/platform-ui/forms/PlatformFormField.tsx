/**
 * PlatformFormField — labelled field wrapper for form controls.
 *
 * Theme-neutral, library-agnostic: it wraps whatever control you pass as
 * children (a React Hook Form-registered input, a select, etc.) with a label,
 * optional help text, and an error message. It deliberately does NOT own form
 * state — the platform standard remains React Hook Form + Zod.
 */

import type { ReactNode } from 'react';

interface PlatformFormFieldProps {
  /** Field id — associates the label with the control. */
  htmlFor: string;
  /** Visible label text. */
  label: string;
  /** The control element (input/select/textarea). */
  children: ReactNode;
  /** Optional helper text shown below the control. */
  hint?: string;
  /** Validation error message (renders invalid feedback when present). */
  error?: string;
  /** Marks the field visually as required. */
  required?: boolean;
  /** Extra classes for the wrapper. */
  className?: string;
}

export function PlatformFormField({
  htmlFor,
  label,
  children,
  hint,
  error,
  required = false,
  className,
}: PlatformFormFieldProps) {
  return (
    <div className={['mb-3', className].filter(Boolean).join(' ')}>
      <label htmlFor={htmlFor} className="form-label">
        {label}
        {required ? <span className="text-danger ms-1" aria-hidden="true">*</span> : null}
      </label>

      {children}

      {hint && !error ? <div className="form-text">{hint}</div> : null}
      {error ? <div className="invalid-feedback d-block">{error}</div> : null}
    </div>
  );
}

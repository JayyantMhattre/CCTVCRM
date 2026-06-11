/**
 * Spinner — loading indicator built on CoreUI / Bootstrap spinner classes.
 *
 * Two variants:
 *  - `fullPage` — fills the entire viewport and centres the spinner;
 *                 used while restoring the auth session on app load.
 *  - Default    — inline spinner for buttons, card loading states, etc.
 *
 * CoreUI classes used:
 *  .spinner-border        — rotating border animation
 *  .spinner-border-sm     — smaller 1rem variant
 *  .text-primary          — uses the CoreUI $primary brand colour
 *  .visually-hidden       — accessible screen-reader text
 */

interface SpinnerProps {
  /** Fill the entire viewport and centre the spinner. */
  fullPage?: boolean;
  /** Accessible label read by screen readers. */
  label?: string;
  size?: 'sm' | 'md' | 'lg';
  /** Override the colour utility class (default: text-primary). */
  color?: string;
}

export function Spinner({
  fullPage = false,
  label    = 'Loading…',
  size     = 'md',
  color    = 'text-primary',
}: SpinnerProps) {
  const sizeClass = size === 'sm' ? ' spinner-border-sm' : '';

  const spinner = (
    <div
      className={`spinner-border ${color}${sizeClass}`}
      role="status"
      aria-label={label}
    >
      <span className="visually-hidden">{label}</span>
    </div>
  );

  if (fullPage) {
    return (
      <div
        className="d-flex flex-column justify-content-center align-items-center gap-3"
        style={{ minHeight: '100vh' }}
        aria-live="polite"
        aria-busy="true"
      >
        {spinner}
        <small className="text-body-secondary">{label}</small>
      </div>
    );
  }

  return spinner;
}

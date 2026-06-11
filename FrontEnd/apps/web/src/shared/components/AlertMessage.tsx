/**
 * AlertMessage — feedback alert using CoreUI / Bootstrap alert classes.
 *
 * Renders a dismissible or static alert banner below forms or inside pages.
 *
 * CoreUI classes used:
 *  .alert .alert-{variant}   — coloured alert container
 *  .alert-dismissible        — adds right-padding for the close button
 *  .btn-close                — accessible × close button
 *
 * Example:
 *   {error   && <AlertMessage variant="danger"  message={extractMessage(error)} />}
 *   {success && <AlertMessage variant="success" message="Saved successfully."  />}
 */

import { CIcon } from '@coreui/icons-react';
import {
  cilCheckCircle,
  cilWarning,
  cilInfo,
  cilXCircle,
} from '@coreui/icons';

interface AlertMessageProps {
  message:  string;
  variant?: 'success' | 'danger' | 'warning' | 'info';
  /**
   * @deprecated Pass the Bootstrap Icons class name here for backward
   * compatibility.  The CoreUI implementation derives the icon from
   * `variant` automatically; this prop is accepted but ignored.
   */
  icon?:    string;
  onClose?: () => void;
}

/** Maps a variant to the appropriate CoreUI icon. */
const variantIcon: Record<NonNullable<AlertMessageProps['variant']>, string[]> = {
  success: cilCheckCircle,
  danger:  cilXCircle,
  warning: cilWarning,
  info:    cilInfo,
};

export function AlertMessage({
  message,
  variant = 'danger',
  onClose,
  icon: _iconIgnored,  // kept for backward compat; CoreUI maps variant → icon
}: AlertMessageProps) {
  const icon = variantIcon[variant];

  return (
    <div
      className={`alert alert-${variant}${onClose ? ' alert-dismissible' : ''} d-flex align-items-center gap-2 mb-3`}
      role="alert"
    >
      {icon && (
        <CIcon icon={icon} className="flex-shrink-0" aria-hidden="true" />
      )}
      <span className="flex-grow-1">{message}</span>
      {onClose && (
        <button
          type="button"
          className="btn-close ms-auto flex-shrink-0"
          aria-label="Close alert"
          onClick={onClose}
        />
      )}
    </div>
  );
}

/**
 * CoreUI adapter — content card.
 *
 * Renders a standard CoreUI `.card` with optional header (title + actions),
 * body, and footer regions.
 */

import type { PlatformCardProps } from '@/theme/contracts';

export function CoreUiCard({
  title,
  actions,
  footer,
  className,
  bodyClassName,
  children,
}: PlatformCardProps) {
  const hasHeader = title !== undefined || actions !== undefined;

  return (
    <div className={['card', 'border-0', 'shadow-sm', className].filter(Boolean).join(' ')}>
      {hasHeader ? (
        <div className="card-header bg-transparent d-flex justify-content-between align-items-center">
          {title !== undefined ? <span className="fw-semibold">{title}</span> : <span />}
          {actions !== undefined ? <div className="d-flex gap-2">{actions}</div> : null}
        </div>
      ) : null}

      <div className={['card-body', bodyClassName].filter(Boolean).join(' ')}>
        {children}
      </div>

      {footer !== undefined ? <div className="card-footer bg-transparent">{footer}</div> : null}
    </div>
  );
}

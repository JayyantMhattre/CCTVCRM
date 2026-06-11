/**
 * HexaDash adapter — content card.
 *
 * Renders the HexaDash "Cards frame" look (10px radius, soft shadow, padded
 * header/body/footer) using token-driven `.hexadash-card*` classes.
 */

import type { PlatformCardProps } from '@/theme/contracts';

export function HexaDashCard({
  title,
  actions,
  footer,
  className,
  bodyClassName,
  children,
}: PlatformCardProps) {
  const hasHeader = title !== undefined || actions !== undefined;

  return (
    <div className={['hexadash-card', className].filter(Boolean).join(' ')}>
      {hasHeader ? (
        <div className="hexadash-card-header">
          {title !== undefined ? <span>{title}</span> : <span />}
          {actions !== undefined ? <div className="d-flex gap-2">{actions}</div> : null}
        </div>
      ) : null}

      <div className={['hexadash-card-body', bodyClassName].filter(Boolean).join(' ')}>
        {children}
      </div>

      {footer !== undefined ? <div className="hexadash-card-footer">{footer}</div> : null}
    </div>
  );
}

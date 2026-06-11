/**
 * HexaDash adapter — breadcrumb.
 *
 * HexaDash-styled breadcrumb (token-driven `.hexadash-breadcrumb*`). Entries
 * with a `to` target render as router links; the targetless entry is the
 * current page (active, `aria-current="page"`).
 */

import { Link } from 'react-router-dom';
import type { PlatformBreadcrumbViewProps } from '@/theme/contracts';

export function HexaDashBreadcrumb({
  items,
  ariaLabel = 'Breadcrumb',
  className,
}: PlatformBreadcrumbViewProps) {
  return (
    <nav aria-label={ariaLabel} className={className}>
      <ol className="hexadash-breadcrumb">
        {items.map((item, index) => {
          const isActive = item.to === undefined;
          return (
            <li
              // Breadcrumb entries are a static, ordered list; index is stable.
              key={item.to ?? index}
              className={['hexadash-breadcrumb-item', isActive ? 'active' : '']
                .filter(Boolean)
                .join(' ')}
              aria-current={isActive ? 'page' : undefined}
            >
              {isActive ? item.label : <Link to={item.to as string}>{item.label}</Link>}
            </li>
          );
        })}
      </ol>
    </nav>
  );
}

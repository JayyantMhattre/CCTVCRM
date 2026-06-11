/**
 * CoreUI adapter — breadcrumb.
 *
 * Renders a Bootstrap/CoreUI `.breadcrumb`. Entries with a `to` target are
 * rendered as router links; the entry without a target (the current page) is
 * rendered active with `aria-current="page"`.
 */

import { Link } from 'react-router-dom';
import type { PlatformBreadcrumbViewProps } from '@/theme/contracts';

export function CoreUiBreadcrumb({
  items,
  ariaLabel = 'Breadcrumb',
  className,
}: PlatformBreadcrumbViewProps) {
  return (
    <nav aria-label={ariaLabel} className={className}>
      <ol className="breadcrumb mb-0">
        {items.map((item, index) => {
          const isActive = item.to === undefined;
          return (
            <li
              // Breadcrumb entries are a static, ordered list; index is stable.
              key={item.to ?? index}
              className={`breadcrumb-item${isActive ? ' active' : ''}`}
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

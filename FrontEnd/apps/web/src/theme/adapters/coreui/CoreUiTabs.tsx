/**
 * CoreUI adapter — tabs.
 *
 * Renders an accessible Bootstrap/CoreUI `.nav-tabs` strip plus a tab-content
 * region. State is owned by the platform (`PlatformTabs`); this component only
 * reflects `activeId` and reports selections via `onChange`.
 */

import type { PlatformTabsViewProps } from '@/theme/contracts';

export function CoreUiTabs({
  items,
  activeId,
  onChange,
  ariaLabel,
  className,
}: PlatformTabsViewProps) {
  return (
    <div className={className}>
      <ul className="nav nav-tabs" role="tablist" aria-label={ariaLabel}>
        {items.map((item) => {
          const isActive = item.id === activeId;
          return (
            <li className="nav-item" role="presentation" key={item.id}>
              <button
                type="button"
                role="tab"
                id={`tab-${item.id}`}
                aria-controls={`tabpanel-${item.id}`}
                aria-selected={isActive}
                tabIndex={isActive ? 0 : -1}
                className={[
                  'nav-link',
                  isActive ? 'active' : '',
                  item.disabled ? 'disabled' : '',
                ]
                  .filter(Boolean)
                  .join(' ')}
                disabled={item.disabled}
                onClick={() => onChange(item.id)}
              >
                {item.label}
              </button>
            </li>
          );
        })}
      </ul>

      <div className="tab-content pt-3">
        {items.map((item) => {
          const isActive = item.id === activeId;
          return (
            <div
              key={item.id}
              role="tabpanel"
              id={`tabpanel-${item.id}`}
              aria-labelledby={`tab-${item.id}`}
              hidden={!isActive}
            >
              {isActive ? item.content : null}
            </div>
          );
        })}
      </div>
    </div>
  );
}

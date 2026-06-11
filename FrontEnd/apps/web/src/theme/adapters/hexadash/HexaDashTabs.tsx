/**
 * HexaDash adapter — tabs.
 *
 * Accessible tab strip + panel using token-driven `.hexadash-tab*` classes.
 * State is owned by the platform (`PlatformTabs`); this only reflects `activeId`
 * and reports selections via `onChange`. No Ant Design `Tabs`.
 */

import type { PlatformTabsViewProps } from '@/theme/contracts';

export function HexaDashTabs({
  items,
  activeId,
  onChange,
  ariaLabel,
  className,
}: PlatformTabsViewProps) {
  return (
    <div className={['hexadash-tabs', className].filter(Boolean).join(' ')}>
      <ul className="hexadash-tabs-strip" role="tablist" aria-label={ariaLabel}>
        {items.map((item) => {
          const isActive = item.id === activeId;
          return (
            <li role="presentation" key={item.id}>
              <button
                type="button"
                role="tab"
                id={`hd-tab-${item.id}`}
                aria-controls={`hd-tabpanel-${item.id}`}
                aria-selected={isActive}
                tabIndex={isActive ? 0 : -1}
                className={['hexadash-tab', isActive ? 'active' : ''].filter(Boolean).join(' ')}
                disabled={item.disabled}
                onClick={() => onChange(item.id)}
              >
                {item.label}
              </button>
            </li>
          );
        })}
      </ul>

      <div className="hexadash-tab-content">
        {items.map((item) => {
          const isActive = item.id === activeId;
          return (
            <div
              key={item.id}
              role="tabpanel"
              id={`hd-tabpanel-${item.id}`}
              aria-labelledby={`hd-tab-${item.id}`}
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

/**
 * HexaDash adapter — sidebar navigation renderer.
 *
 * Renders the platform-supplied, *resolved* navigation model as a HexaDash-style
 * vertical menu (token-driven `.hexadash-nav*` classes). Makes NO access
 * decisions — it renders only `visible` content, preserving separator placement
 * exactly like the CoreUI adapter.
 */

import { Fragment } from 'react';
import { NavLink } from 'react-router-dom';
import { CIcon } from '@coreui/icons-react';
import type { NavigationItem, PlatformNavProps } from '@/theme/contracts';
import { HEXADASH_NAV_ICONS } from './navIcons';

function NavLeaf({ item }: { item: NavigationItem }) {
  return (
    <li>
      <NavLink
        to={item.to}
        className={({ isActive }) =>
          `hexadash-nav-link${isActive ? ' active' : ''}`
        }
      >
        {item.icon ? (
          <CIcon
            icon={HEXADASH_NAV_ICONS[item.icon]}
            className="hexadash-nav-icon"
            aria-hidden="true"
          />
        ) : null}
        <span>{item.label}</span>
        {item.badge ? (
          <span className="hexadash-badge hexadash-badge-primary ms-auto">
            {item.badge.label}
          </span>
        ) : null}
      </NavLink>
    </li>
  );
}

export function HexaDashNav({ groups, ariaLabel = 'Main navigation' }: PlatformNavProps) {
  return (
    <ul className="hexadash-nav" aria-label={ariaLabel}>
      {groups.map((group, index) => (
        <Fragment key={group.id}>
          {group.visible && group.title ? (
            <li className="hexadash-nav-title" aria-hidden="true">
              {group.title}
            </li>
          ) : null}

          {group.visible
            ? group.items
                .filter((item) => item.visible)
                .map((item) => <NavLeaf key={item.id} item={item} />)
            : null}

          {index < groups.length - 1 ? (
            <li className="hexadash-nav-divider" role="separator" />
          ) : null}
        </Fragment>
      ))}
    </ul>
  );
}

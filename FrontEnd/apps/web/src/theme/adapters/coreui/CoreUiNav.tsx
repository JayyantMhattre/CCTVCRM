/**
 * CoreUI adapter — sidebar navigation renderer.
 *
 * Renders a CoreUI `.sidebar-nav` list from the platform-supplied, *resolved*
 * navigation model. Produces markup identical to the pre-T3 inline sidebar:
 *   - `<li class="nav-title">` per visible group heading
 *   - `<li class="nav-item"><NavLink class="nav-link">…</NavLink></li>` per item
 *   - `<li class="nav-divider">` between adjacent groups
 *
 * Separator parity: a divider is emitted after every group except the last,
 * regardless of that group's visibility — exactly as the previous markup placed
 * its dividers as siblings of the (sometimes hidden) guarded sections.
 *
 * This component makes NO access decisions — it renders only `visible` content.
 */

import { Fragment } from 'react';
import { NavLink } from 'react-router-dom';
import { CIcon } from '@coreui/icons-react';
import type { NavigationItem, PlatformNavProps } from '@/theme/contracts';
import { CORE_UI_NAV_ICONS } from './navIcons';

function NavLeaf({ item }: { item: NavigationItem }) {
  return (
    <li className="nav-item">
      <NavLink
        to={item.to}
        className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}
      >
        {item.icon ? (
          <CIcon icon={CORE_UI_NAV_ICONS[item.icon]} className="nav-icon" aria-hidden="true" />
        ) : null}
        <span>{item.label}</span>
      </NavLink>
    </li>
  );
}

export function CoreUiNav({ groups, ariaLabel = 'Main navigation' }: PlatformNavProps) {
  return (
    <ul className="sidebar-nav" aria-label={ariaLabel}>
      {groups.map((group, index) => (
        <Fragment key={group.id}>
          {group.visible && group.title ? (
            <li className="nav-title text-uppercase fw-semibold" aria-hidden="true">
              {group.title}
            </li>
          ) : null}

          {group.visible
            ? group.items
                .filter((item) => item.visible)
                .map((item) => <NavLeaf key={item.id} item={item} />)
            : null}

          {index < groups.length - 1 ? (
            <li className="nav-divider" role="separator" />
          ) : null}
        </Fragment>
      ))}
    </ul>
  );
}

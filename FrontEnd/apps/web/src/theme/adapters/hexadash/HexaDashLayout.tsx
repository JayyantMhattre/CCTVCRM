/**
 * HexaDash adapter — authenticated application shell (owns all visual markup).
 *
 * Replicates the HexaDash shell language (280px fixed sider, 64px sticky header,
 * content area, footer; mobile overlay below 991px, auto-collapse below 1200px)
 * using CoreUI/Bootstrap-compatible primitives styled by the extracted HexaDash
 * tokens. NO Ant Design `Layout`, NO Redux theme slice.
 *
 * Navigation is supplied as a fully-resolved render model (`navGroups`) by the
 * platform and rendered via `HexaDashNav` — this adapter makes ZERO access
 * decisions (no roles/permissions/routes-knowledge/menu definitions).
 *
 * Dark mode is self-managed via `data-hexadash-mode` on the theme wrapper,
 * independent of CoreUI's `data-coreui-theme` mechanism.
 *
 * This module also imports the HexaDash skin stylesheet (scoped under
 * `.hexadash-theme`, so it is inert while CoreUI is active).
 */

import { Outlet, useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { CIcon } from '@coreui/icons-react';
import { cilMenu, cilAccountLogout, cilSun, cilMoon, cilChevronLeft } from '@coreui/icons';
import type { AuthUser } from '@/shared/types/auth.types';
import type { PlatformLayoutProps, NavigationGroup } from '@/theme/contracts';
import { HexaDashNav } from './HexaDashNav';
import { HexaDashAvatar } from './HexaDashAvatar';
import './hexadash.scss';

const TABLET_BREAKPOINT = 1200;

export function HexaDashLayout({ user, onLogout, appName, navGroups }: PlatformLayoutProps) {
  const navigate = useNavigate();

  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false);
  const [sidebarNarrow, setSidebarNarrow] = useState(
    () => typeof window !== 'undefined' && window.innerWidth <= TABLET_BREAKPOINT,
  );

  const [darkMode, setDarkMode] = useState<boolean>(
    () =>
      typeof window !== 'undefined' &&
      window.matchMedia('(prefers-color-scheme: dark)').matches,
  );

  // Close the mobile sidebar on route change.
  useEffect(() => {
    setMobileSidebarOpen(false);
  }, [navigate]);

  // Dark-mode bridge: module page content is CoreUI-styled (the global
  // `coreui.scss` is always loaded and keys dark mode off `data-coreui-theme`).
  // Mirroring the HexaDash toggle onto that attribute keeps the shell AND the
  // page interior consistently light/dark. Adapter-scoped: this layout only
  // mounts when HexaDash is the active theme.
  useEffect(() => {
    document.documentElement.setAttribute(
      'data-coreui-theme',
      darkMode ? 'dark' : 'light',
    );
  }, [darkMode]);

  const sidebarClasses = [
    'hexadash-sidebar',
    sidebarNarrow ? 'hexadash-sidebar-narrow' : '',
    mobileSidebarOpen ? 'hexadash-sidebar-open' : '',
  ]
    .filter(Boolean)
    .join(' ');

  const mainClasses = ['hexadash-main', sidebarNarrow ? 'hexadash-main-narrow' : '']
    .filter(Boolean)
    .join(' ');

  return (
    <div className="hexadash-theme" data-hexadash-mode={darkMode ? 'dark' : 'light'}>
      <div className="hexadash-shell">
        {/* ── Sidebar ──────────────────────────────────────────────────────── */}
        <aside className={sidebarClasses} id="hexadash-sidebar">
          <SidebarContent
            narrow={sidebarNarrow}
            onToggleNarrow={() => setSidebarNarrow((prev) => !prev)}
            user={user}
            onLogout={onLogout}
            appName={appName}
            navGroups={navGroups}
          />
        </aside>

        {/* Backdrop closes the mobile sidebar when tapped */}
        {mobileSidebarOpen ? (
          <div
            className="hexadash-backdrop d-lg-none"
            onClick={() => setMobileSidebarOpen(false)}
            aria-hidden="true"
          />
        ) : null}

        {/* ── Main column ─────────────────────────────────────────────────── */}
        <div className={mainClasses}>
          <header className="hexadash-header">
            <button
              className="btn btn-link p-0 d-lg-none"
              type="button"
              onClick={() => setMobileSidebarOpen((prev) => !prev)}
              aria-label="Toggle navigation menu"
              aria-expanded={mobileSidebarOpen}
              style={{ color: 'var(--hd-heading-color)' }}
            >
              <CIcon icon={cilMenu} size="lg" aria-hidden="true" />
            </button>

            <span
              className="fw-semibold d-none d-sm-inline"
              style={{ color: 'var(--hd-heading-color)' }}
            >
              {appName}
            </span>

            <div className="ms-auto d-flex align-items-center gap-3">
              {import.meta.env.DEV
                ? user?.roles.map((role) => (
                    <span key={role} className="hexadash-badge hexadash-badge-light d-none d-md-inline">
                      {role}
                    </span>
                  ))
                : null}

              <button
                className="btn btn-link p-0"
                type="button"
                onClick={() => setDarkMode((prev) => !prev)}
                aria-label={darkMode ? 'Switch to light mode' : 'Switch to dark mode'}
                title={darkMode ? 'Light mode' : 'Dark mode'}
                style={{ color: 'var(--hd-heading-color)' }}
              >
                <CIcon icon={darkMode ? cilSun : cilMoon} size="lg" aria-hidden="true" />
              </button>

              {user ? (
                <div className="d-flex align-items-center gap-2">
                  <HexaDashAvatar name={user.displayName} size="sm" />
                  <span
                    className="small fw-semibold d-none d-md-inline text-truncate"
                    style={{ maxWidth: '120px', color: 'var(--hd-heading-color)' }}
                  >
                    {user.displayName}
                  </span>
                </div>
              ) : null}
            </div>
          </header>

          <main className="hexadash-content">
            <Outlet />
          </main>

          <footer className="hexadash-footer">
            <span>
              &copy; {new Date().getFullYear()} {appName}
            </span>
            <span className="d-none d-sm-inline">Powered by HexaDash design language</span>
          </footer>
        </div>
      </div>
    </div>
  );
}

// ── SidebarContent ───────────────────────────────────────────────────────────

function SidebarContent({
  narrow,
  onToggleNarrow,
  user,
  onLogout,
  appName,
  navGroups,
}: {
  narrow: boolean;
  onToggleNarrow: () => void;
  user: AuthUser | null;
  onLogout: () => void;
  appName: string;
  navGroups: readonly NavigationGroup[];
}) {
  return (
    <>
      <a className="hexadash-sidebar-brand" href="/" aria-label="Go to dashboard">
        {/* Reuse the avatar circle for a compact 2rem brand mark; the larger
            `.hexadash-brand-mark` is reserved for the auth screen. */}
        <span className="hexadash-avatar hexadash-avatar-sm" aria-hidden="true">
          {appName.charAt(0).toUpperCase()}
        </span>
        {!narrow ? <span>{appName}</span> : null}
        <button
          className="btn btn-link p-0 ms-auto"
          type="button"
          onClick={(event) => {
            event.preventDefault();
            onToggleNarrow();
          }}
          aria-label={narrow ? 'Expand sidebar' : 'Collapse sidebar'}
          style={{ color: 'var(--hd-menu-icon-color)' }}
        >
          <CIcon icon={cilChevronLeft} className={narrow ? 'rotate-180' : ''} aria-hidden="true" />
        </button>
      </a>

      <div className="hexadash-sidebar-body">
        <HexaDashNav groups={navGroups} ariaLabel="Main navigation" />
      </div>

      <div className="hexadash-sidebar-footer">
        {!narrow && user ? (
          <div className="d-flex align-items-center gap-2 mb-2">
            <HexaDashAvatar name={user.displayName} size="sm" />
            <div className="overflow-hidden">
              <div className="fw-semibold text-truncate small" style={{ color: 'var(--hd-heading-color)' }}>
                {user.displayName}
              </div>
              <div className="text-truncate" style={{ fontSize: '0.7rem', color: 'var(--hd-text-color)' }}>
                {user.email}
              </div>
            </div>
          </div>
        ) : null}

        <button
          className="btn btn-sm w-100 d-flex align-items-center justify-content-center gap-2"
          type="button"
          onClick={onLogout}
          aria-label="Sign out"
          style={{
            border: '1px solid var(--hd-border-normal)',
            color: 'var(--hd-text-color-secondary)',
          }}
        >
          <CIcon icon={cilAccountLogout} aria-hidden="true" />
          {!narrow ? <span>Sign out</span> : null}
        </button>
      </div>
    </>
  );
}

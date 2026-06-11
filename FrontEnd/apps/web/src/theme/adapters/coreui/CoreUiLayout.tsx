/**
 * CoreUI adapter — authenticated application shell (owns all visual markup).
 *
 * T3 (navigation migration): the sidebar no longer defines menus or evaluates
 * `RoleGuard` / `PermissionGuard`. Navigation is supplied as a fully-resolved
 * render model (`navGroups`) by the platform and rendered via `CoreUiNav`. This
 * adapter contains ZERO business-navigation logic — no roles, permissions,
 * routes-knowledge or menu definitions.
 *
 * Behaviour (sidebar/header/footer/dark-mode) is identical to the pre-T3 shell:
 *   - fixed sidebar (desktop) + overlay drawer (mobile), narrow toggle, CSS var
 *   - sticky header with dark-mode toggle and user info
 *   - footer
 *
 * Dark mode remains self-managed here (CoreUI-specific `data-coreui-theme`).
 */

import { Outlet, useNavigate } from 'react-router-dom';
import { useState, useEffect, useCallback }  from 'react';
import { CIcon }            from '@coreui/icons-react';
import {
  cilMenu,
  cilAccountLogout,
  cilSun,
  cilMoon,
  cilChevronLeft,
}                           from '@coreui/icons';
import type { AuthUser }    from '@/shared/types/auth.types';
import type { PlatformLayoutProps } from '@/theme/contracts';
import type { NavigationGroup } from '@/theme/contracts';
import { CoreUiNav } from './CoreUiNav';

// ── Constants ──────────────────────────────────────────────────────────────

/** Width of the sidebar in CSS units — matches $sidebar-width in coreui.scss */
const SIDEBAR_WIDTH = '16rem';

/** Width of the narrow (icon-only) sidebar */
const SIDEBAR_NARROW_WIDTH = '4rem';

// ── Main layout ────────────────────────────────────────────────────────────

export function CoreUiLayout({ user, onLogout, appName, navGroups }: PlatformLayoutProps) {
  const navigate = useNavigate();

  // ── Sidebar state ────────────────────────────────────────────────────────
  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false);
  const [sidebarNarrow, setSidebarNarrow]          = useState(false);

  // ── Dark mode state (self-managed by the CoreUI theme) ───────────────────
  const [darkMode, setDarkMode] = useState<boolean>(
    () => document.documentElement.getAttribute('data-coreui-theme') === 'dark'
      || window.matchMedia('(prefers-color-scheme: dark)').matches,
  );

  const updateSidebarCssVar = useCallback((narrow: boolean) => {
    document.documentElement.style.setProperty(
      '--cui-sidebar-occupy-start',
      narrow ? SIDEBAR_NARROW_WIDTH : SIDEBAR_WIDTH,
    );
  }, []);

  useEffect(() => {
    updateSidebarCssVar(sidebarNarrow);
  }, [sidebarNarrow, updateSidebarCssVar]);

  useEffect(() => {
    document.documentElement.setAttribute(
      'data-coreui-theme',
      darkMode ? 'dark' : 'light',
    );
  }, [darkMode]);

  // Close mobile sidebar on route change.
  useEffect(() => {
    setMobileSidebarOpen(false);
  }, [navigate]);

  function toggleNarrow() {
    setSidebarNarrow((prev) => !prev);
  }

  const sidebarClasses = [
    'sidebar',
    'sidebar-dark',
    'sidebar-fixed',
    'border-end',
    'd-none',
    'd-lg-flex',
    'flex-column',
    mobileSidebarOpen ? 'show' : '',
    sidebarNarrow     ? 'sidebar-narrow' : '',
  ]
    .filter(Boolean)
    .join(' ');

  const mobileSidebarClasses = [
    'sidebar',
    'sidebar-dark',
    'sidebar-fixed',
    'border-end',
    'd-flex',
    'd-lg-none',
    'flex-column',
    mobileSidebarOpen ? 'show' : 'hide',
  ]
    .filter(Boolean)
    .join(' ');

  return (
    <>
      {/* ── Desktop sidebar ──────────────────────────────────────────────── */}
      <div className={sidebarClasses} id="sidebar">
        <SidebarContent
          narrow={sidebarNarrow}
          onToggleNarrow={toggleNarrow}
          user={user}
          onLogout={onLogout}
          appName={appName}
          navGroups={navGroups}
        />
      </div>

      {/* ── Mobile sidebar (overlay drawer) ──────────────────────────────── */}
      <div className={mobileSidebarClasses} id="sidebar-mobile" style={{ zIndex: 1050 }}>
        <SidebarContent
          narrow={false}
          onToggleNarrow={() => setMobileSidebarOpen(false)}
          user={user}
          onLogout={onLogout}
          appName={appName}
          navGroups={navGroups}
        />
      </div>

      {/* Backdrop that closes the mobile sidebar when tapped */}
      {mobileSidebarOpen && (
        <div
          className="sidebar-backdrop fade show d-lg-none"
          style={{ zIndex: 1040 }}
          onClick={() => setMobileSidebarOpen(false)}
          aria-hidden="true"
        />
      )}

      {/* ── Main wrapper (shifts right to account for sidebar) ───────────── */}
      <div className="wrapper d-flex flex-column min-vh-100">

        {/* ── Sticky header / topbar ─────────────────────────────────────── */}
        <header className="header header-sticky p-0 mb-4">
          <div className="container-fluid border-bottom px-4">

            {/* Mobile hamburger — opens the mobile overlay sidebar */}
            <button
              className="header-toggler d-lg-none me-2"
              type="button"
              onClick={() => setMobileSidebarOpen((prev) => !prev)}
              aria-label="Toggle navigation menu"
              aria-expanded={mobileSidebarOpen}
            >
              <CIcon icon={cilMenu} size="lg" aria-hidden="true" />
            </button>

            {/* App name / breadcrumb (left-aligned on desktop) */}
            <span className="fw-semibold text-body ms-2 d-none d-sm-inline">
              {appName}
            </span>

            {/* Spacer pushes right-side controls to the far right */}
            <div className="ms-auto d-flex align-items-center gap-3">

              {/* Dev-mode role badge */}
              {import.meta.env.DEV && user?.roles.map((role) => (
                <span key={role} className="badge bg-secondary d-none d-md-inline">
                  {role}
                </span>
              ))}

              {/* Dark / light mode toggle */}
              <button
                className="btn btn-link text-body p-0"
                type="button"
                onClick={() => setDarkMode((prev) => !prev)}
                aria-label={darkMode ? 'Switch to light mode' : 'Switch to dark mode'}
                title={darkMode ? 'Light mode' : 'Dark mode'}
              >
                <CIcon
                  icon={darkMode ? cilSun : cilMoon}
                  size="lg"
                  aria-hidden="true"
                />
              </button>

              {/* User avatar + name */}
              {user && (
                <div className="d-flex align-items-center gap-2">
                  <div
                    className="avatar-sm bg-primary text-white"
                    title={user.displayName}
                  >
                    {user.displayName.charAt(0).toUpperCase()}
                  </div>
                  <span className="small fw-semibold d-none d-md-inline text-truncate" style={{ maxWidth: '120px' }}>
                    {user.displayName}
                  </span>
                </div>
              )}
            </div>
          </div>
        </header>

        {/* ── Page content ──────────────────────────────────────────────── */}
        <div className="body flex-grow-1 px-3">
          <div className="container-lg px-4">
            {/* React Router renders the matched module page here */}
            <div className="page-fade-in">
              <Outlet />
            </div>
          </div>
        </div>

        {/* ── Footer ────────────────────────────────────────────────────── */}
        <footer className="footer px-4 py-2 d-flex align-items-center justify-content-between">
          <span className="small text-muted">
            &copy; {new Date().getFullYear()}{' '}
            <a href="/" className="text-muted text-decoration-none fw-semibold">
              {appName}
            </a>
          </span>
          <span className="small text-muted d-none d-sm-inline">
            Powered by CoreUI &amp; React
          </span>
        </footer>
      </div>
    </>
  );
}

// ── SidebarContent (shared between desktop + mobile instances) ─────────────

function SidebarContent({
  narrow,
  onToggleNarrow,
  user,
  onLogout,
  appName,
  navGroups,
}: {
  narrow:          boolean;
  onToggleNarrow:  () => void;
  user:            AuthUser | null;
  onLogout:        () => void;
  appName:         string;
  navGroups:       readonly NavigationGroup[];
}) {
  return (
    <>
      {/* ── Sidebar header (brand + narrow toggler) ──────────────────────── */}
      <div className="sidebar-header border-bottom d-flex align-items-center justify-content-between px-3">
        <a className="sidebar-brand text-decoration-none" href="/" aria-label="Go to dashboard">
          {/* Show full brand name OR just initials in narrow mode */}
          {!narrow ? (
            <span className="sidebar-brand-full fw-bold text-white fs-5">
              {appName}
            </span>
          ) : (
            <span className="sidebar-brand-narrow fw-bold text-white fs-5 mx-auto">
              A
            </span>
          )}
        </a>

        {/* Toggler button (desktop: collapse to narrow; mobile: close) */}
        <button
          className="sidebar-toggler"
          type="button"
          onClick={onToggleNarrow}
          aria-label={narrow ? 'Expand sidebar' : 'Collapse sidebar'}
        >
          <CIcon
            icon={cilChevronLeft}
            className={narrow ? 'rotate-180' : ''}
            aria-hidden="true"
          />
        </button>
      </div>

      {/* ── Navigation links (platform-supplied, theme-rendered) ─────────── */}
      <div className="sidebar-body d-flex flex-column flex-grow-1 overflow-y-auto">
        <CoreUiNav groups={navGroups} ariaLabel="Main navigation" />
      </div>

      {/* ── Sidebar footer — user info + sign-out ───────────────────────── */}
      <div className="sidebar-footer border-top px-3 py-3 mt-auto">
        {!narrow && user && (
          <div className="d-flex align-items-center gap-2 mb-2">
            <div className="avatar-sm bg-primary text-white flex-shrink-0">
              {user.displayName.charAt(0).toUpperCase()}
            </div>
            <div className="overflow-hidden">
              <div className="text-white fw-semibold text-truncate small">
                {user.displayName}
              </div>
              <div className="text-muted text-truncate" style={{ fontSize: '0.7rem' }}>
                {user.email}
              </div>
            </div>
          </div>
        )}

        <button
          className="btn btn-sm btn-outline-light w-100 d-flex align-items-center justify-content-center gap-2"
          type="button"
          onClick={onLogout}
          aria-label="Sign out"
        >
          <CIcon icon={cilAccountLogout} aria-hidden="true" />
          {!narrow && <span>Sign out</span>}
        </button>
      </div>
    </>
  );
}

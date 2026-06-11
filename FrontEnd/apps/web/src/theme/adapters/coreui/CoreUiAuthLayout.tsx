/**
 * CoreUI adapter — unauthenticated (auth pages) shell (owns visual markup).
 *
 * T2 (layout migration): the auth shell markup that previously lived in
 * `layouts/AuthLayout.tsx` now lives here. The auth *gating* (loading spinner /
 * redirect-when-authenticated) is platform concern and lives in the
 * `PlatformAuthLayout` orchestrator — this component only renders the shell.
 *
 * Behaviour/appearance is identical to the pre-T2 `AuthLayout`.
 */

import { Outlet } from 'react-router-dom';
import type { PlatformAuthLayoutProps } from '@/theme/contracts';

export function CoreUiAuthLayout({ appName, children }: PlatformAuthLayoutProps) {
  return (
    <div
      className="d-flex flex-column justify-content-center align-items-center min-vh-100 bg-body-secondary"
      style={{
        background: 'linear-gradient(135deg, var(--cui-primary-bg-subtle) 0%, var(--cui-body-bg) 100%)',
        padding: '1.5rem',
      }}
    >
      {/* ── Brand logotype ─────────────────────────────────────────────── */}
      <div className="text-center mb-4">
        <div
          className="d-inline-flex align-items-center justify-content-center rounded-3 mb-3"
          style={{
            width: '3.5rem',
            height: '3.5rem',
            background: 'var(--cui-primary)',
          }}
          aria-hidden="true"
        >
          <span className="text-white fw-bold fs-3">
            {appName.charAt(0).toUpperCase()}
          </span>
        </div>

        <h1 className="h3 fw-bold text-body mb-1">{appName}</h1>
        <p className="text-body-secondary small mb-0">Modular SaaS Platform</p>
      </div>

      {/* ── Auth card ──────────────────────────────────────────────────── */}
      <div className="card border-0 shadow-lg w-100" style={{ maxWidth: '420px' }}>
        <div className="card-body p-4">
          {children ?? <Outlet />}
        </div>
      </div>

      {/* ── Footer ─────────────────────────────────────────────────────── */}
      <footer className="text-center text-body-secondary small mt-4">
        &copy; {new Date().getFullYear()} {appName} Platform. All rights reserved.
      </footer>
    </div>
  );
}

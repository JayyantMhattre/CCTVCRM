/**
 * HexaDash adapter — unauthenticated (auth pages) shell.
 *
 * Centred auth card on the HexaDash gradient background. Renders the visual
 * shell only — the auth *gating* (loading/redirect) stays in the platform
 * `PlatformAuthLayout`. No auth logic, no API, no routing decisions here.
 */

import { Outlet } from 'react-router-dom';
import type { PlatformAuthLayoutProps } from '@/theme/contracts';

export function HexaDashAuthLayout({ appName, children }: PlatformAuthLayoutProps) {
  return (
    <div className="hexadash-theme hexadash-auth min-vh-100">
      <div className="text-center mb-4">
        <div className="hexadash-brand-mark mb-3" aria-hidden="true">
          {appName.charAt(0).toUpperCase()}
        </div>
        <h1 className="h3 fw-bold mb-1" style={{ color: 'var(--hd-heading-color)' }}>
          {appName}
        </h1>
        <p className="small mb-0" style={{ color: 'var(--hd-text-color)' }}>
          Modular SaaS Platform
        </p>
      </div>

      <div className="hexadash-auth-card">{children ?? <Outlet />}</div>

      <footer className="text-center small mt-4" style={{ color: 'var(--hd-text-color)' }}>
        &copy; {new Date().getFullYear()} {appName} Platform. All rights reserved.
      </footer>
    </div>
  );
}

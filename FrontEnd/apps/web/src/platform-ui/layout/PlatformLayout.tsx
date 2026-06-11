/**
 * PlatformLayout — theme-agnostic authenticated application shell.
 *
 * Thin orchestration layer:
 *   - provides the resolved navigation model (`PlatformNavigationProvider`)
 *   - gathers platform-owned data the shell needs (current user, logout handler,
 *     product name, navigation groups)
 *   - delegates ALL rendering to the active theme adapter's `Layout`
 *
 *   Router → PlatformLayout → Theme Adapter → CoreUI Layout
 *
 * It contains no markup of its own. All navigation *decisions* happen in the
 * platform (the provider/resolver); the theme renders the resolved model only.
 */

import { useNavigate } from 'react-router-dom';
import { useTheme } from '@/theme';
import { useAuth } from '@/shared/hooks/useAuth';
import { ROUTES } from '@/core/router/routeMap';
import { PlatformNavigationProvider, useNavigationModel } from '@/platform-ui/navigation';

export function PlatformLayout() {
  return (
    <PlatformNavigationProvider>
      <PlatformShell />
    </PlatformNavigationProvider>
  );
}

function PlatformShell() {
  const { adapter } = useTheme();
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const navGroups = useNavigationModel();

  function handleLogout() {
    logout();
    void navigate(ROUTES.login, { replace: true });
  }

  const Layout = adapter.layout.Layout;

  return (
    <Layout
      navGroups={navGroups}
      user={user}
      onLogout={handleLogout}
      appName={import.meta.env.VITE_APP_NAME ?? 'Ashraak'}
    />
  );
}

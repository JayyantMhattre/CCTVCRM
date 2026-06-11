/**
 * navigationConfig — the platform's single, declarative menu definition.
 *
 * This is the source of truth for *what* the application navigation contains and
 * *who* may see each part (via `visibility` rules). It is intentionally free of
 * any theme/UI-library concern: icons are referenced by theme-agnostic keys, and
 * routes come from the central `ROUTES` map.
 *
 * The link set, ordering, sections and visibility rules below reproduce the
 * pre-T3 sidebar exactly:
 *   - General → Dashboard
 *   - Tenant  → Tenant Profile, Tenant Settings
 *   - Users   → User List, My Profile        (roles: Admin or Manager)
 *   - Audit   → Audit Logs                    (permission: audit:read)
 */

import { ROUTES } from '@/core/router/routeMap';
import type { NavigationGroupConfig } from './models';

export const NAVIGATION_CONFIG: readonly NavigationGroupConfig[] = [
  {
    id: 'general',
    title: 'General',
    items: [
      { id: 'dashboard', label: 'Dashboard', to: ROUTES.dashboard, icon: 'dashboard' },
    ],
  },
  {
    id: 'tenant',
    title: 'Tenant',
    items: [
      { id: 'tenant-profile', label: 'Tenant Profile', to: ROUTES.tenant.profile, icon: 'tenant' },
      { id: 'tenant-settings', label: 'Tenant Settings', to: ROUTES.tenant.settings, icon: 'tenant-settings' },
    ],
  },
  {
    id: 'users',
    title: 'Users',
    visibility: { roles: ['Admin', 'Manager'] },
    items: [
      { id: 'users-list', label: 'User List', to: ROUTES.users.list, icon: 'users' },
      // Matches the pre-T3 sidebar link target exactly.
      { id: 'users-me', label: 'My Profile', to: ROUTES.users.profile, icon: 'profile' },
    ],
  },
  {
    id: 'audit',
    title: 'Audit',
    visibility: { permissions: ['audit:read'] },
    items: [
      { id: 'audit-logs', label: 'Audit Logs', to: ROUTES.audit.logs, icon: 'audit' },
    ],
  },
];

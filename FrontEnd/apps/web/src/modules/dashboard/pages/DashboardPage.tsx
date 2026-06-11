/**
 * DashboardPage — home screen for authenticated users.
 *
 * Uses CoreUI's `.card-stat` helper class (defined in coreui.scss) for
 * the quick-access tiles and standard CoreUI card structure throughout.
 *
 * Quick-access stat cards use:
 *   .card-stat  — subtle shadow card without border (defined in coreui.scss)
 *   .card-body  — padded content region
 *   CIcon       — CoreUI SVG icon via @coreui/icons-react
 *
 * Production: replace the static tiles with real TanStack Query data hooks
 * once the backend exposes summary/stats endpoints.
 */

import { Link }           from 'react-router-dom';
import { CIcon }          from '@coreui/icons-react';
import {
  cilBuilding,
  cilPeople,
  cilDescription,
  cilSettings,
  cilShare,
  cilBug,
  cilLockLocked,
}                         from '@coreui/icons';
import { useAuth }         from '@/shared/hooks/useAuth';
import { useTenant }       from '@/shared/hooks/useTenant';
import { RoleGuard }       from '@/shared/guards/RoleGuard';
import { PermissionGuard } from '@/shared/guards/PermissionGuard';
import { PageHeader }      from '@/shared/components/PageHeader';
import { ROUTES }          from '@/core/router/routeMap';

// ── Stat card type ─────────────────────────────────────────────────────────

interface StatCard {
  to:         string;
  icon:       string[];
  label:      string;
  sub:        string;
  iconBg:     string;   // CoreUI bg-* utility
  iconColor:  string;   // CoreUI text-* utility
}

export default function DashboardPage() {
  const { user }     = useAuth();
  const { tenantId } = useTenant();

  // ── Quick-access module cards ────────────────────────────────────────────
  const statCards: StatCard[] = [
    {
      to:        ROUTES.tenant.profile,
      icon:      cilBuilding,
      label:     'Tenant',
      sub:       'View profile',
      iconBg:    'bg-primary bg-opacity-10',
      iconColor: 'text-primary',
    },
    {
      to:        ROUTES.tenant.settings,
      icon:      cilSettings,
      label:     'Settings',
      sub:       'Tenant configuration',
      iconBg:    'bg-info bg-opacity-10',
      iconColor: 'text-info',
    },
  ];

  return (
    <div>
      <PageHeader
        title={`Welcome back, ${user?.displayName ?? 'there'}`}
        subtitle="Here's what's happening in your workspace today."
      />

      {/* ── Quick-access stat tiles ──────────────────────────────────────── */}
      <div className="row g-3 mb-4">
        {/* Always-visible cards */}
        {statCards.map((card) => (
          <div key={card.to} className="col-sm-6 col-xl-3">
            <Link to={card.to} className="text-decoration-none">
              <div className="card-stat card">
                <div className="card-body d-flex align-items-center gap-3">
                  <div className={`rounded-3 ${card.iconBg} p-3`}>
                    <CIcon icon={card.icon} size="xl" className={card.iconColor} aria-hidden="true" />
                  </div>
                  <div>
                    <div className="fw-semibold text-body">{card.label}</div>
                    <div className="text-body-secondary small">{card.sub}</div>
                  </div>
                </div>
              </div>
            </Link>
          </div>
        ))}

        {/* Users card — Admin / Manager only */}
        <RoleGuard roles={['Admin', 'Manager']} inline>
          <div className="col-sm-6 col-xl-3">
            <Link to={ROUTES.users.list} className="text-decoration-none">
              <div className="card-stat card">
                <div className="card-body d-flex align-items-center gap-3">
                  <div className="rounded-3 bg-success bg-opacity-10 p-3">
                    <CIcon icon={cilPeople} size="xl" className="text-success" aria-hidden="true" />
                  </div>
                  <div>
                    <div className="fw-semibold text-body">Users</div>
                    <div className="text-body-secondary small">Manage members</div>
                  </div>
                </div>
              </div>
            </Link>
          </div>
        </RoleGuard>

        {/* API Keys card — apikeys:read permission */}
        <PermissionGuard permission="apikeys:read">
          <div className="col-sm-6 col-xl-3">
            <Link to={ROUTES.apikeys.list} className="text-decoration-none">
              <div className="card-stat card">
                <div className="card-body d-flex align-items-center gap-3">
                  <div className="rounded-3 bg-secondary bg-opacity-10 p-3">
                    <CIcon icon={cilLockLocked} size="xl" className="text-secondary" aria-hidden="true" />
                  </div>
                  <div>
                    <div className="fw-semibold text-body">API Keys</div>
                    <div className="text-body-secondary small">Manage access keys</div>
                  </div>
                </div>
              </div>
            </Link>
          </div>
        </PermissionGuard>

        {/* Webhooks card — webhooks:read permission */}
        <PermissionGuard permission="webhooks:read">
          <div className="col-sm-6 col-xl-3">
            <Link to={ROUTES.webhooks.overview} className="text-decoration-none">
              <div className="card-stat card">
                <div className="card-body d-flex align-items-center gap-3">
                  <div className="rounded-3 bg-info bg-opacity-10 p-3">
                    <CIcon icon={cilShare} size="xl" className="text-info" aria-hidden="true" />
                  </div>
                  <div>
                    <div className="fw-semibold text-body">Webhooks</div>
                    <div className="text-body-secondary small">Operations center</div>
                  </div>
                </div>
              </div>
            </Link>
          </div>
        </PermissionGuard>

        {/* Audit card — audit:read permission only */}
        <PermissionGuard permission="audit:read">
          <div className="col-sm-6 col-xl-3">
            <Link to={ROUTES.audit.logs} className="text-decoration-none">
              <div className="card-stat card">
                <div className="card-body d-flex align-items-center gap-3">
                  <div className="rounded-3 bg-warning bg-opacity-10 p-3">
                    <CIcon icon={cilDescription} size="xl" className="text-warning" aria-hidden="true" />
                  </div>
                  <div>
                    <div className="fw-semibold text-body">Audit</div>
                    <div className="text-body-secondary small">View activity log</div>
                  </div>
                </div>
              </div>
            </Link>
          </div>
        </PermissionGuard>
      </div>

      {/* ── Session debug info panel (dev mode only) ─────────────────────── */}
      {import.meta.env.DEV && (
        <div className="card">
          <div className="card-header bg-transparent d-flex align-items-center gap-2 fw-semibold">
            <CIcon icon={cilBug} className="text-warning" aria-hidden="true" />
            Session Debug Info
            <span className="badge bg-warning text-dark ms-1">DEV ONLY</span>
          </div>
          <div className="card-body p-0">
            <table className="table table-sm table-borderless mb-0">
              <tbody>
                <tr>
                  <th className="w-25 ps-3">User ID</th>
                  <td className="font-monospace small">{user?.userId}</td>
                </tr>
                <tr>
                  <th className="ps-3">Email</th>
                  <td>{user?.email}</td>
                </tr>
                <tr>
                  <th className="ps-3">Tenant ID</th>
                  <td className="font-monospace small">{tenantId}</td>
                </tr>
                <tr>
                  <th className="ps-3">Roles</th>
                  <td>
                    {user?.roles.map((r) => (
                      <span key={r} className="badge bg-primary me-1">{r}</span>
                    ))}
                  </td>
                </tr>
                <tr>
                  <th className="ps-3">Permissions</th>
                  <td>
                    {user?.permissions.length
                      ? user.permissions.map((p) => (
                          <span key={p} className="badge bg-secondary me-1 mb-1">{p}</span>
                        ))
                      : <span className="text-body-secondary small">none</span>
                    }
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}

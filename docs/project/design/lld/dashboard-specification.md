# Dashboard Specification

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7 — widget-level LLD (extends [dashboard-design.md](../dashboard-design.md))

Each widget: **Purpose** · **Data source (API)** · **Refresh** · **Permissions** · **Class** · **Platform components**

---

## 1. Admin Dashboard (`/admin` — screen #11)

| Widget | Purpose | Data source | Refresh | Permission | Class | Components |
|--------|---------|-------------|---------|------------|:-----:|------------|
| Lead pipeline summary | Counts per lead status | `GET /admin/dashboard` → `leadPipeline` | On mount + 5 min interval | `leads:read` | NEW | `PlatformCard` + badge row |
| AMC expiry radar | 30/60/90 day buckets | `→ amcExpiry` | 5 min | `amc:read` | NEW | `PlatformCard` + 3 count tiles |
| Visit schedule today/week | Planned/Assigned/InProgress/Missed | `→ visitSchedule` | 2 min | `schedules:read` | NEW | `PlatformCard` + `PlatformBadge` |
| Pending report approvals | Queue count + oldest age | `→ pendingApprovals` | 1 min | `visits:approve` | NEW | `PlatformCard` + link to #28 |
| Open tickets by priority | 4 priority counts | `→ ticketsByPriority` | 2 min | `tickets:read` | NEW | `PlatformCard` + grid |
| Invoice status summary | Draft/Generated/Sent/Paid + unpaid total | `→ invoiceSummary` | 5 min | `invoices:read` | NEW | `PlatformCard` |
| Engineer workload | Top 5 engineers by active assignments | `→ engineerWorkload` | 5 min | `engineers:read` | NEW | `PlatformTable` mini |
| Recent audit activity | Last 5 audit entries | `GET /audit-logs?pageSize=5` | On mount | `audit:read` | **REUSE** | Platform audit tile |
| Webhook/API key health | Failed deliveries / key expiry | Platform dashboard APIs | On mount | `webhooks:read` / `apikeys:read` | **REUSE** | Platform cards |
| Quick actions | Shortcuts to create flows | Static routes | — | per action perm | NEW | Button group in `PlatformCard` |

**Page class:** EXTEND platform dashboard shell · **Layout:** 3-column responsive grid (2-col tablet, 1-col mobile)

### Quick action routes

| Action | Route | Permission |
|--------|-------|------------|
| New Lead | `/admin/leads/new` | `leads:manage` |
| New Ticket | `/admin/tickets/new` | `tickets:create` |
| New Invoice | `/admin/invoices/new` | `invoices:manage` |
| Schedule Visit | `/admin/schedules` | `schedules:manage` |

---

## 2. Customer Dashboard (`/portal` — screen #46)

| Widget | Purpose | Data source | Refresh | Permission | Class | Components |
|--------|---------|-------------|---------|------------|:-----:|------------|
| AMC status card | Plan, validity, days to expiry | `GET /portal/dashboard` → `amcStatus` | On mount + pull refresh | `amc:read` | NEW | `PlatformCard` |
| Renewal CTA | Button when expiry ≤ 60 days | same + business rule | On mount | `amc:request-renewal` | NEW | Primary button in card |
| Next scheduled visit | Nearest upcoming | `→ nextVisit` | On mount | `schedules:read` | NEW | `PlatformCard` |
| Service history snapshot | Last 3 approved visits | `→ recentVisits` | On mount | `visits:read` | NEW | Mini list |
| Open tickets | Count + recent 3 | `→ openTickets` | On mount | `tickets:read` | NEW | `PlatformCard` + link #51 |
| Invoices due | Unpaid Generated/Sent | `→ dueInvoices` | On mount | `invoices:read` | NEW | `PlatformCard` |
| Notifications feed | Recent 5 events | `GET /portal/notifications?limit=5` | On mount + push | role | **EXTEND** | List in `PlatformCard` |
| Profile/security | Links to profile, sessions | Static | — | — | **REUSE** | Platform profile links |

**Mobile:** Same API; Flutter tab dashboard with pull-to-refresh.

---

## 3. Engineer Dashboard (`/engineer` — screen #59)

| Widget | Purpose | Data source | Refresh | Permission | Class | Components |
|--------|---------|-------------|---------|------------|:-----:|------------|
| My Day | Today's visits list | `GET /engineer/dashboard` → `todayVisits` | On mount + 2 min | `schedules:read` | NEW | `PlatformTable` / list |
| Visit checklist state | In-progress visit progress | `→ activeVisitChecklist` | 30 sec if active | `visits:execute` | NEW | **CctvEvidenceChecklist** |
| Assigned tickets | Open count + list | `→ assignedTickets` | 2 min | `tickets:read` | NEW | `PlatformCard` |
| Returned reports | Count needing rework | `→ returnedReports` | On mount | `visits:read` | NEW | Alert `PlatformBadge` |
| Offline sync status | Pending queue count | Local sync state + `POST sync` result | Real-time (mobile) | role | NEW | Banner (mobile-primary) |
| Profile/security | Profile link | Static | — | — | **REUSE** | Platform links |

---

## 4. Refresh behavior standards

| Context | Strategy |
|---------|----------|
| Web dashboard | TanStack Query `staleTime` per widget + manual browser refresh |
| After mutation | Invalidate dashboard query keys from mutation `onSuccess` |
| Mobile | Pull-to-refresh on dashboard tabs |
| Background | No WebSocket V1 — polling intervals above |

---

## 5. Empty / error states

| State | UX |
|-------|-----|
| No data | Widget shows "No items" + contextual action link |
| Partial permission | Widget hidden entirely (`PermissionGuard`) |
| API error | Widget-level error text + retry button; no full page crash |
| Loading | Skeleton inside `PlatformCard` (adapter-themed) |

---

## 6. Classification summary

| Class | Admin | Customer | Engineer |
|-------|:-----:|:--------:|:--------:|
| REUSE | 2 | 1 | 1 |
| EXTEND | 1 | 1 | 0 |
| NEW | 7 | 6 | 4 |

---

Related: [dashboard-design.md](../dashboard-design.md) · [report-specification.md](./report-specification.md) · [platform-component-reuse.md](./platform-component-reuse.md)

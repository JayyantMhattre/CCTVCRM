# Navigation Architecture

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-5 — RBAC, Navigation & Screen Inventory (base-template aware)
**Platform shell (REUSE):** centralized router (`core/router`), lazy routes, `AuthGuard`/`RoleGuard`/`PermissionGuard`, theme-adapter layout/navigation contracts ([routing-and-guards](../../frontend/routing-and-guards.md), [theme decision record](../../frontend/themes/theme-decision-record.md))

> Navigation is rendered by the **existing platform shell** through the theme NavigationContract — CCTV adds **route definitions and nav items**, never a new shell, never a theme import. Classification legend: **REUSE** (existing platform nav as-is) · **EXTEND** (existing nav item/shell with CCTV context) · **NEW** (new nav item/route group).

---

## 1. Shell strategy

| Concern | Approach | Class |
|---------|----------|-------|
| App shell, sidebar, header, layout | Platform Theme Engine (`platform-ui` + Layout/Navigation contracts) | REUSE |
| Route registration | Central router + lazy module pages (platform pattern) | REUSE (mechanism) |
| Role-based portal entry | Login → role claim → redirect: Admin → `/admin`, Engineer → `/engineer`, Customer → `/portal` | EXTEND (existing RoleGuard, new route trees) |
| Nav item visibility | `PermissionGuard` per item (existing inline mode) | REUSE (mechanism) |
| Public website | New public route tree (anonymous) sharing the SPA or static site fronting www.aarvii.in | NEW |

## 2. Public Website navigation

| Menu item | Route | Class | Notes |
|-----------|-------|-------|-------|
| Home | `/` | NEW | Reuse existing www.aarvii.in content (freeze §2) |
| About Us | `/about` | NEW | Content reuse |
| Services | `/services` | NEW | Content reuse |
| AMC Services | `/amc-services` | NEW | Plan information |
| Get Quote | `/get-quote` | NEW | Inquiry → auto-lead (BR-LEAD-02) |
| AMC Inquiry | `/amc-inquiry` | NEW | Inquiry → auto-lead |
| Contact Us | `/contact` | NEW | Inquiry → auto-lead |
| Gallery | `/gallery` | NEW | Content reuse |
| Testimonials | `/testimonials` | NEW | Content reuse |
| Login | `/login` | **REUSE** | Existing platform login page + flows (MFA, OTP, password reset) |

## 3. Admin Portal navigation (summary — detail in [admin-portal-navigation.md](./admin-portal-navigation.md))

| Menu item | Class | Basis |
|-----------|-------|-------|
| Dashboard | EXTEND | Existing dashboard page extended with CCTV widgets ([dashboard-design.md](./dashboard-design.md)) |
| Leads | NEW | `leads:read` |
| Customers (+ Sites, Assets) | NEW | `customers:read` |
| AMC (Plans, Contracts) | NEW | `amcplans:read` / `amc:read` |
| Scheduling | NEW | `schedules:read` |
| Visit Reports (approval queue) | NEW | `visits:approve` |
| Tickets | NEW | `tickets:read` |
| Engineers | NEW | `engineers:read` |
| Invoices | NEW | `invoices:read` |
| Reports | NEW | `reports:read` |
| **Administration** group | **REUSE** | Existing platform nav: Users, Tenant Profile/Settings, Audit Logs (`audit:read`), API Keys (`apikeys:read`), Webhooks (`webhooks:read`), Sessions, Profile |

## 4. Customer Portal navigation (summary — detail in [customer-portal-navigation.md](./customer-portal-navigation.md))

| Menu item | Class | Basis |
|-----------|-------|-------|
| Dashboard | NEW (page) on REUSE shell | Customer-scoped widgets |
| My AMC | NEW | `amc:read` (active term only) |
| Visits & Service History | NEW | `schedules:read`, `visits:read` (approved) |
| My Tickets | NEW | `tickets:read/create/reopen` |
| My Invoices | NEW | `invoices:read/download` |
| Profile & Security | **REUSE** | Platform profile, password reset (OTP), sessions, notification preferences |

## 5. Engineer Portal navigation (summary — detail in [engineer-portal-navigation.md](./engineer-portal-navigation.md))

| Menu item | Class | Basis |
|-----------|-------|-------|
| My Day / Home | NEW (page) on REUSE shell | Assigned work summary |
| Assigned Visits | NEW | `schedules:read` (assigned) |
| Visit Reporting (within visit detail) | NEW | `visits:execute`; uploads via platform Files (REUSE) |
| Assigned Tickets | NEW | `tickets:read` (assigned) + `tickets:create` |
| Profile & Security | **REUSE** | Platform profile/sessions/password |

## 6. Classification totals

| Class | Items |
|-------|-------|
| REUSE | Login/auth flows, entire Administration group (7 platform areas), Profile & Security on both portals, shell/layout/guards everywhere |
| EXTEND | Admin dashboard, role-based login redirect |
| NEW | Public site tree (9), Admin business tree (9 groups), Customer tree (5), Engineer tree (4) |

## 7. Route namespace plan

```
/                      → public website (anonymous)
/login, /register      → platform auth (REUSE)
/admin/...             → Admin portal (RoleGuard: Admin)
/portal/...            → Customer portal (RoleGuard: Customer)
/engineer/...          → Engineer portal (RoleGuard: Engineer)
/403, *                → platform error pages (REUSE)
```

Existing platform routes (`/users`, `/audit`, `/tenant/*`, webhooks, api-keys) mount under the Admin portal's Administration group unchanged.

---

Related: [screen-inventory.md](./screen-inventory.md) · [rbac-matrix.md](./rbac-matrix.md) · portal details: [admin](./admin-portal-navigation.md) · [customer](./customer-portal-navigation.md) · [engineer](./engineer-portal-navigation.md)

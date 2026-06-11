# Mobile navigation

**Decision:** [GoRouter](../adr/ADR-Mobile-0002-navigation.md)

---

## Principles

1. **Declarative routes** — URL paths mirror web where sensible (`/login`, `/dashboard`).
2. **Auth guards** — redirect unauthenticated users to login; preserve intended route.
3. **Role guards** — admin-only routes (audit) check JWT roles.
4. **Deep links** — same path scheme for universal links / app links (M2+).

---

## Planned route table

| Path | Feature | Auth | Role |
|------|---------|------|------|
| `/login` | auth | Public | — |
| `/register` | auth | Public | — |
| `/mfa` | auth | Partial | — |
| `/dashboard` | shell | Required | — |
| `/tenant` | tenant | Required | — |
| `/tenant/settings` | tenant | Required | TenantAdmin |
| `/users` | users | Required | Admin, Manager |
| `/users/:id` | users | Required | — |
| `/account/preferences` | users | Required | Self |
| `/account/sessions` | sessions | Required | Self |
| `/audit` | audit | Required | Admin |
| `/files` | files | Required | — |

Implementation: `lib/core/navigation/app_router.dart` (M1).

---

## Shell pattern

Authenticated area uses a **shell route** (bottom nav or drawer) wrapping child routes — analogous to web `AppLayout`.

---

## MFA flow

```
/login → (mfa_required) → /mfa → /dashboard
```

Matches backend `grant_type=mfa` after password grant.

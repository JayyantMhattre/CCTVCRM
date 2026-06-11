# Webhook permissions (mobile)

## Required permission

`webhooks:read` — parsed from JWT `permission` claim (same as web).

`webhooks:manage` also grants read access for operators who manage subscriptions on web.

## Enforcement

| Layer | Behavior |
|-------|----------|
| GoRouter redirect | Webhook routes redirect to `/home` without permission |
| Drawer | Webhooks nav tile hidden |
| Home module card | Hidden without permission |

## Implementation

- `JwtClaims.hasPermission()`
- `CurrentUser.canReadWebhooks`

No role-only fallback — permission claim is required.

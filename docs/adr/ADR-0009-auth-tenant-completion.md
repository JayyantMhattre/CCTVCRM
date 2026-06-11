# ADR-0009: Auth and tenant completion phase

## Status

Accepted

## Decision

- Invitations as Auth aggregate with contract `UserInvitedEvent` for Notifications.
- TOTP MFA with password + `mfa` grant for login completion.
- `auth.user_sessions` for device management.
- `GET/PATCH` tenant settings and `PATCH` user preferences APIs.
- MongoDB `AuditReadService` for paginated audit queries.

## Consequences

- New tables: `auth.invitations`, `auth.user_sessions`.
- `ITenantService.GetSettingsAsync` for cross-module MFA policy.
- Frontend audit viewer uses real paginated API.

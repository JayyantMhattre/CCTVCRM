# Mobile — Sessions module

**Feature:** `FrontEnd.Mobile/lib/features/sessions/`

## APIs

| Action | Method | Path |
|--------|--------|------|
| List | `GET` | `/api/v1/auth/sessions` |
| Revoke one | `POST` | `/api/v1/auth/sessions/{sessionId}/revoke` |
| Revoke all | `POST` | `/api/v1/auth/sessions/revoke-all` |

## Display

| Field | Source |
|-------|--------|
| Created | `createdOnUtc` |
| Last used | `lastUsedOnUtc` |
| Device | Parsed from `userAgent` |
| IP | `ipAddress` |
| Status | `isRevoked` → Active / Revoked |

Revoked sessions filtered from list. Successful revoke/revoke-all shown via success toast.

## Provider

`SessionsProvider` — list, revoke, revoke-all.

## Route

`/sessions`

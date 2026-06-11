# Mobile — API Keys visibility

**Feature:** `FrontEnd.Mobile/lib/features/apikeys/`

Read-only key metadata and usage — no create, rotate, or revoke.

## Routes

| Path | Page |
|------|------|
| `/api-keys` | List + usage summary |
| `/api-keys/:id` | Detail |

## Permission

`apikeys:read` or `apikeys:manage` via JWT `permission` claim.

## APIs

- `GET /api/v1/api-keys`
- `GET /api/v1/api-keys/{id}`

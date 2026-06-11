# API Keys admin UI (web)

**Module:** `FrontEnd/apps/web/src/modules/apikeys/`

## Routes

| Path | Page |
|------|------|
| `/api-keys` | List, search, create |
| `/api-keys/:id` | Detail, usage, rotate, revoke |

## Permissions

| Permission | Access |
|------------|--------|
| `apikeys:read` | View list and detail |
| `apikeys:manage` | Create, rotate, revoke, scope updates |

See [permissions.md](./permissions.md).

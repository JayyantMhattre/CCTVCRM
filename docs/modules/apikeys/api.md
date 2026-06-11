# API Keys REST API

Base: `/api/v1/api-keys` (JWT required for management)

## Endpoints

| Method | Path | Permission | Description |
|--------|------|------------|-------------|
| GET | `/` | `apikeys:read` | List tenant keys |
| GET | `/{id}` | `apikeys:read` | Key detail + usage |
| POST | `/` | `apikeys:manage` | Create (returns plaintext once) |
| POST | `/{id}/rotate` | `apikeys:manage` | Rotate secret |
| POST | `/{id}/revoke` | `apikeys:manage` | Revoke key |
| PUT | `/{id}/scopes` | `apikeys:manage` | Update scopes |
| GET | `/health` | Anonymous | Module health |

## M2M authentication

Send key via:

- `X-API-Key: ashk_prod_...`
- `Authorization: Bearer ashk_prod_...`

Configurable in `ApiKeys` appsettings section.

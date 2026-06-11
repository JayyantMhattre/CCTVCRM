# Sessions & devices

Persistent sessions in `auth.user_sessions`, listed via API.

| Method | Path |
|--------|------|
| GET | `/api/v1/auth/sessions` |
| POST | `/api/v1/auth/sessions/{id}/revoke` |
| POST | `/api/v1/auth/sessions/revoke-all` |

Revoke raises `TokenRevokedDomainEvent` / `RevokeAllSessionsDomainEvent` (audit).

Frontend: `/account/sessions` (React Sessions page).

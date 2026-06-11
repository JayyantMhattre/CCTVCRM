# Auth and Session

## Components

| File | Role |
|------|------|
| `core/auth/authStore.ts` | Zustand — tokens, user, sessionStorage |
| `core/auth/tokenService.ts` | Decode JWT, refresh token |
| `core/auth/AuthProvider.tsx` | Restore session on mount |
| `modules/auth/api.ts` | login, register API calls |
| `modules/auth/pages/LoginPage.tsx` | Password grant flow |

---

## Login flow

1. User submits email, password, tenantId
2. `authApi.login` → `POST /connect/token` (form-urlencoded)
3. `tokenService.mapTokenToUser` decodes access token
4. `authStore.setSession` — persists to `sessionStorage` key `ashraak_session`
5. Navigate to dashboard or `state.from`

---

## Token refresh

`core/api/interceptors.ts`:

- On 401, calls `tokenService.refresh()` once
- On failure: `clearSession()` + redirect `/login`

Refresh uses `grant_type=refresh_token` against `/connect/token`.

---

## JWT claims used

| Claim | Usage |
|-------|-------|
| `sub` | userId |
| `email` | display |
| `tenant_id` / `tenantId` | tenant scope |
| `role` | RoleGuard (array) |
| `permission` | PermissionGuard (array) |

---

## Security notes

- Tokens stored in **sessionStorage** (survives refresh, cleared on tab close)
- `withCredentials: true` on Axios for cookie-ready SSO future path
- SSO UI not wired — endpoints exist in `endpoints.ts`

---

## Related

- [api/auth.md](../api/auth.md)
- [api-layer.md](./api-layer.md)

# API Layer

## Axios client

`FrontEnd/apps/web/src/core/api/client.ts`

```typescript
baseURL: import.meta.env.VITE_API_BASE_URL
timeout: 15000
withCredentials: true
```

---

## Endpoints catalog

`FrontEnd/apps/web/src/core/api/endpoints.ts`

| Group | Key routes |
|-------|------------|
| auth | `/connect/token`, `/api/v1/auth/register`, SSO paths |
| tenants | `/api/v1/tenants/current`, `/tenants/current/settings`, provision |
| users | `/api/v1/users/...`, `/users/{id}/preferences` |
| audit | `/api/v1/audit-logs` |

Version segment from `VITE_API_VERSION` (default `v1`).

---

## Interceptors

`FrontEnd/apps/web/src/core/api/interceptors.ts`

Registered in `AppProviders` at startup.

| Direction | Behavior |
|-----------|----------|
| Request | Bearer token + `X-Correlation-Id` |
| Response 401 | Refresh + retry once |
| Response error | Classify → global toast (except 401); optional `_skipErrorToast` |

---

## Module API pattern

Each feature module exports functions:

```typescript
// modules/users/api.ts
export const usersApi = {
  getById: (id: string) => apiClient.get(ENDPOINTS.users.byId(id)),
};
```

Use TanStack Query in pages:

```typescript
useQuery({ queryKey: ['user', id], queryFn: () => usersApi.getById(id) });
```

---

## Dev proxy

`vite.config.ts` proxies `/api` and `/connect` to `VITE_API_BASE_URL` — avoids CORS in local dev.

---

## Error handling

- `shared/errors/apiErrorClassifier.ts` — validation, auth, rate limit, server, network
- `shared/hooks/useApiError.ts` — same classifier for inline alerts
- Global toasts via interceptor — see [errors/README.md](./errors/README.md)

---

## Related

- [api/overview.md](../api/overview.md)
- [errors/problem-details.md](../errors/problem-details.md)
- [toasts/README.md](./toasts/README.md)
- [correlation-support/README.md](./correlation-support/README.md)

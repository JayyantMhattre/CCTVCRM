# API Keys admin permissions

Uses `PermissionGuard` and `useApiKeyPermissions()` hook.

- Route guard: `ApiKeysRouteGuard` — requires `apikeys:read` or `apikeys:manage`
- Mutations hidden without `apikeys:manage`
- Sidebar and dashboard card gated on `apikeys:read`

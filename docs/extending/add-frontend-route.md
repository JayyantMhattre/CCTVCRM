# Add a Frontend Route

React 19 SPA — routes are centralized in `FrontEnd/apps/web/src/core/router/index.tsx`.

---

## Steps

### 1. Create module folder

```
src/modules/yourfeature/
├── api.ts
├── types.ts
└── pages/
    └── YourPage.tsx
```

### 2. Add API functions

Use `apiClient` and `ENDPOINTS` from `core/api/`.

Extend `endpoints.ts` with new paths under `/api/v1/...`.

### 3. Add route constant

`core/router/routeMap.ts`:

```typescript
yourfeature: {
  list: '/yourfeature',
},
```

### 4. Register route

`core/router/index.tsx`:

```tsx
const YourPage = lazy(() => import('@/modules/yourfeature/pages/YourPage'));

// Inside AuthGuard > AppLayout children:
{ path: ROUTES.yourfeature.list, element: <Page><YourPage /></Page> },
```

### 5. Add navigation (optional)

`layouts/AppLayout.tsx` — wrap with `PermissionGuard` or `RoleGuard` as needed.

### 6. Document

- Update `docs/frontend/feature-modules.md`
- Add feature section or `docs/frontend/yourfeature.md` if large

---

## Guards

| Need | Use |
|------|-----|
| Logged in | Parent `AuthGuard` |
| Role | `RoleGuard` on route or inline |
| Permission | `PermissionGuard` inline |

---

## Related

- [frontend/routing-and-guards.md](../frontend/routing-and-guards.md)
- [DEVELOPER_GUIDE.md](../../DEVELOPER_GUIDE.md) §5.9

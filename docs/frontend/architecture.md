# Frontend Architecture

Package: `@ashraak/web` at `FrontEnd/apps/web/`.

**Stack:** React 19, TypeScript 5 (strict), Vite 6, React Router 7, TanStack Query 5, Zustand 5, Axios, CoreUI 5.

> This template uses **React**, not Angular. Use these docs for frontend work.

---

## Entry flow

```
index.html → main.tsx → App.tsx → AppProviders → AppRouter
```

- `AppProviders`: QueryClient + `AuthProvider` + `ToastContainer`
- `AppRouter`: `createBrowserRouter` + lazy-loaded pages

---

## Layer responsibilities

| Layer | Path | Responsibility |
|-------|------|----------------|
| Core | `src/core/` | API, auth, routing — no feature rules |
| Modules | `src/modules/` | Feature pages and API wrappers |
| Shared | `src/shared/` | Guards, hooks, toast, errors, reusable UI |
| Layouts | `src/layouts/` | App shell and auth shell |

---

## Styling

- `styles/coreui.scss` — CoreUI theme
- `styles/_variables.scss` — SCSS variables
- Dark mode toggle in `AppLayout` (local state)

See [COREUI_INTEGRATION.md](../../FrontEnd/COREUI_INTEGRATION.md).

---

## Monorepo

- Root: `FrontEnd/package.json` — scripts delegate to `@ashraak/web`
- Workspace: `pnpm-workspace.yaml` — `apps/*` only (`packages/ui` planned, not present)

---

## Related

- [routing-and-guards.md](./routing-and-guards.md)
- [architecture/frontend-architecture.md](../architecture/frontend-architecture.md)

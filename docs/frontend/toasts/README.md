# Global toast system

Centralized user feedback for the React SPA (CoreUI-styled toasts).

**Source:** `FrontEnd/apps/web/src/shared/ui/toast/`

## Components

| File | Role |
|------|------|
| `toastStore.ts` | Zustand queue + `toastService` (imperative API) |
| `ToastContainer.tsx` | Fixed top-right stack |
| `useToast.ts` | React hook |

## Variants

`success` · `warning` · `error` · `info`

## Mounting

`ToastContainer` is rendered in `AppProviders` (sibling to router children).

## Related

- [usage.md](./usage.md)
- [extending.md](./extending.md)
- [errors](../errors/README.md)

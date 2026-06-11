# Toasts — Extending

## Add a variant

1. Extend `ToastVariant` in `toast.types.ts`
2. Map icon + background in `ToastContainer.tsx`
3. Add helper on `toastService` if needed

## Persist toasts across navigation

Toasts are global and survive route changes (container lives in `AppProviders`).

## Do not

- Introduce a second notification library (react-hot-toast, etc.)
- Mount multiple `ToastContainer` instances

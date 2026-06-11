# Toasts — Usage

## In React components

```tsx
import { useToast } from '@/shared/ui/toast';

function MyPage() {
  const toast = useToast();

  function onSaved() {
    toast.success('Changes saved.');
  }

  function onWarning() {
    toast.warning('This action cannot be undone.');
  }
}
```

## Outside React (interceptors)

```ts
import { toastService } from '@/shared/ui/toast';

toastService.error('Server unavailable.', {
  title: 'Connection problem',
  correlationId: 'abc123',
  autoDismissMs: 8000,
});
```

## Options

| Option | Default | Description |
|--------|---------|-------------|
| `autoDismissMs` | 5000 | 0 = manual dismiss only |
| `title` | Variant name | Toast header |
| `correlationId` | — | Shown with copy button (API errors) |

## Queue

Maximum **5** visible toasts; oldest dropped when exceeded.

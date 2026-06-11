# Error classification

`classifyApiError(error)` returns:

```typescript
{
  category: 'validation' | 'auth' | ...;
  variant: ToastVariant;
  title: string;
  message: string;
  correlationId: string | null;
  status?: number;
}
```

## Message priority

1. RFC 7807 `detail`
2. RFC 7807 `title`
3. Axios `message`
4. Generic fallback

## Skipping global toast

Per-request flag:

```typescript
apiClient.get('/optional', { _skipErrorToast: true });
```

Used when a 404 is expected (e.g. tenant settings GET before backend exposes the route).

## Page-level errors

`useApiError()` wraps the classifier for inline `AlertMessage` + `CorrelationIdCopy` on full-page error states.

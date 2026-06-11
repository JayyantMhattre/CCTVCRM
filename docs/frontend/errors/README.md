# API error UX

Human-readable API failures via classification + global toasts.

**Classifier:** `FrontEnd/apps/web/src/shared/errors/apiErrorClassifier.ts`

**Interceptor:** `FrontEnd/apps/web/src/core/api/interceptors.ts`

## Categories

| Category | Typical status | Toast variant |
|----------|----------------|---------------|
| validation | 400, 422 | warning |
| auth | 401 | (no toast — refresh/redirect) |
| forbidden | 403 | warning |
| notFound | 404 | info |
| rateLimit | 429 | warning (+ Retry-After hint) |
| server | 5xx | error |
| network | no response | warning |

## Correlation ID

Failed responses surface `X-Correlation-Id` in the toast body with a copy button.

## Related

- [error-classification.md](./error-classification.md)
- [interceptor-flow.md](./interceptor-flow.md)
- [correlation-support](../correlation-support/README.md)

# Correlation ID support (frontend)

End-to-end alignment with backend `X-Correlation-Id` middleware.

**Module:** `FrontEnd/apps/web/src/shared/errors/correlationId.ts`

## Request

Every API call receives `X-Correlation-Id` (preserved if already set, otherwise `crypto.randomUUID()` without hyphens).

## Response

The latest ID from response headers is stored via `setLastCorrelationId` for support workflows.

## User-visible surfaces

| Surface | Behaviour |
|---------|-----------|
| Error toasts | Correlation ID + copy button |
| Full-page errors | `CorrelationIdCopy` below `AlertMessage` |
| Audit / settings pages | Same on query failures |

## Related

- [support-workflow.md](./support-workflow.md)
- [Backend correlation](../../platform/correlation/README.md)

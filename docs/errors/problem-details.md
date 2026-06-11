# Problem Details (RFC 7807)

Ashraak registers global exception handling:

```csharp
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
```

Implementation: `BackEnd/src/Host/Ashraak.Api/Middleware/GlobalExceptionHandler.cs`

---

## When ProblemDetails is returned

| Source | Status | Example |
|--------|--------|---------|
| Unhandled exception | 500 | NullReference in handler |
| `Results.Problem(...)` | As specified | Register validation failure (400) |
| Authorization middleware | 401/403 | Standard ASP.NET Core |

---

## Typical shape

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Email is already registered for this tenant."
}
```

Exact `type` URI may vary by ASP.NET version.

---

## OAuth vs ProblemDetails

| Endpoint family | Error format |
|-----------------|--------------|
| `/connect/token` | `{ error, error_description }` |
| `/api/v1/*` | ProblemDetails for app-generated errors |

Frontend `useApiError` should handle both if SSO/token flows expand.

---

## Client handling (React)

```typescript
// Axios error
const detail = error.response?.data?.detail ?? error.response?.data?.title;
```

---

## Related

- [error-catalog.md](./error-catalog.md)
- [api/overview.md](../api/overview.md)

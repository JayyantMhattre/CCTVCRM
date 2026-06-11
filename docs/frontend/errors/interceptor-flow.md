# Interceptor flow

```mermaid
sequenceDiagram
    participant App
    participant Req as Request interceptor
    participant API as Ashraak.Api
    participant Res as Response interceptor
    participant Toast as toastService

    App->>Req: HTTP call
    Req->>Req: Bearer token + X-Correlation-Id
    Req->>API: Request
    alt 401 first attempt
        API-->>Res: 401
        Res->>Res: refresh token + retry once
    else other error
        API-->>Res: 4xx/5xx
        Res->>Res: classifyApiError
        Res->>Toast: show (except auth)
    end
    Res-->>App: reject promise
```

## Auth 401

1. Silent refresh via `tokenService.refresh()`
2. Retry original request once (`_retry` flag)
3. On refresh failure → `clearSession()` + redirect `/login` (no toast)

## Request correlation

If the caller did not set `X-Correlation-Id`, the request interceptor generates one and stores it via `setLastCorrelationId`.

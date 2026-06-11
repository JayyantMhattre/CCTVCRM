# API client

Shared HTTP stack in `lib/core/api/`. All features use `baseApiClientProvider` or generated SDK wrappers — never raw `Dio` in features.

## BaseApiClient

Thin facade over configured `Dio` with `get` / `post` helpers. Base URL: `{apiBaseUrl}/api/v1` from `EnvironmentConfig`.

## Interceptor chain (order matters)

| Interceptor | Responsibility |
|-------------|----------------|
| `CorrelationInterceptor` | Adds `X-Correlation-Id`; logs locally |
| `AuthInterceptor` | Bearer access token from `authSessionProvider` |
| `LoggingInterceptor` | Request/response debug (verbose in Dev/QA) |
| `RetryInterceptor` | Transient failure retries |
| `RefreshTokenInterceptor` | 401 → refresh via `/connect/token` → retry |

## Refresh flow

On 401, `RefreshTokenInterceptor` posts to `EnvironmentConfig.tokenUrl` (not under `/api/v1`) with `grant_type=refresh_token`. New tokens persist via `TokenStorage` and update `authSessionProvider`.

Uses a separate `Dio` instance for refresh to avoid interceptor recursion.

## Correlation

Aligns with [platform correlation](../../platform/correlation/README.md):

- Header: `X-Correlation-Id`
- Generated UUID (hex, no dashes) when absent
- Stored in `lastCorrelationId` for error UX (M2+)

## Generated SDK

Manual DTOs are forbidden. Run:

```powershell
./scripts/generate-mobile-sdk.ps1
```

Output: `FrontEnd.Mobile/packages/api_client/`. Wrap generated client in `core/api/` adapters — do not edit generated files.

See [ADR-Mobile-0005](../../adr/ADR-Mobile-0005-openapi-sdk-generation.md).

# API Keys architecture

## Module layout

```
BackEnd/src/Modules/ApiKeys/
├── Ashraak.ApiKeys.Domain/       — ApiKey aggregate, domain events
├── Ashraak.ApiKeys.Application/  — Commands, queries, authorization
├── Ashraak.ApiKeys.Infrastructure/ — EF, Argon2, middleware, metrics
└── Ashraak.ApiKeys.Api/          — Minimal API endpoints
```

Contracts: `Ashraak.SharedKernel.Contracts/ApiKeys/`

## Data model

PostgreSQL schema `apikeys`, table `api_keys`:

- Tenant-scoped with global query filter
- `key_prefix` for indexed lookup
- `hashed_secret` — Argon2id only
- `scopes` — JSON array of `resource:action` permissions
- Usage counters: `request_count`, `success_count`, `failure_count`

## Authentication flow

1. Client sends `X-API-Key` or `Bearer ashk_*`
2. `ApiKeyAuthenticationMiddleware` validates via `IApiKeyValidator`
3. Claims principal populated with `tenant_id` + scope permissions
4. `ApiKeyUsageMiddleware` records usage after response

Management endpoints (`/api/v1/api-keys`) require JWT — API key auth is bypassed.

## Audit

Domain events auto-audit via `DomainEventAuditHandler`:

- `ApiKeyCreated`, `ApiKeyRotated`, `ApiKeyRevoked`, `ApiKeyScopesChanged`, `ApiKeyUsed`

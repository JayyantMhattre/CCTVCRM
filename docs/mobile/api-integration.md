# Mobile API integration

## Rule

Mobile uses the **same backend APIs** as React web. **No mobile-specific endpoints.**

Base path: `/api/v1/` (versioned) + `/connect/token` (OAuth).

---

## Client stack (planned)

| Piece | Choice |
|-------|--------|
| HTTP | `dio` (interceptors, multipart, cancel tokens) |
| Auth | OAuth2 password + refresh; MFA grant |
| Serialization | Generated Dart models from OpenAPI |
| Errors | RFC 7807 ProblemDetails (same as web) |

---

## Headers (every request)

| Header | Source |
|--------|--------|
| `Authorization` | `Bearer {access_token}` |
| `X-Tenant-Id` or claim | Match web tenant resolution |
| `X-Correlation-Id` | Generated per request; echo on errors |

---

## Endpoints by feature

See [module-map.md](./module-map.md) and `docs/api/`.

Interactive reference (dev): `http://localhost:5000/scalar/v1`

---

## OpenAPI strategy

| Phase | Action |
|-------|--------|
| M1 | Document OpenAPI export from host (Scalar/OpenAPI) |
| M1 | Add CI job: OpenAPI → Dart SDK |
| M2 | Web TypeScript SDK from same spec (optional parity) |

**Governance:** OpenAPI spec is the contract source; hand-written DTOs are forbidden for API payloads.

Details: [sdk-generation.md](./sdk-generation.md).

---

## Environments

| Flavor | API base (example) |
|--------|-------------------|
| Dev | `http://10.0.2.2:5000` (Android emulator) / `http://localhost:5000` (iOS sim) |
| QA | `https://api-qa.example.com` |
| UAT | `https://api-uat.example.com` |
| Prod | `https://api.example.com` |

Configured via `--dart-define` or flavor-specific `config/` — see [release-process.md](./release-process.md).

Secrets (client IDs if any) via CI secrets — never committed.

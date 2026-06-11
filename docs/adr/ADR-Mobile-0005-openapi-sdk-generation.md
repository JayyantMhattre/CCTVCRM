# ADR-Mobile-0005: OpenAPI SDK generation (mobile)

## Status

Accepted (M1)

## Context

Mobile governance forbids hand-written API DTOs. Backend OpenAPI is the HTTP projection of `SharedKernel.Contracts`.

## Options evaluated

| Option | Pros | Cons |
|--------|------|------|
| **openapi-generator `dart-dio`** | Dio-aligned output, Docker reproducible | Generated code verbosity |
| **swagger_dart_code_generator** | Dart-native | Less common in enterprise CI |
| **Manual models in features** | Fast initially | Drift, violates governance |

## Decision

Generate Dart client with **OpenAPI Generator** (`dart-dio` target) into `FrontEnd.Mobile/packages/api_client/`.

Scripts: `scripts/generate-mobile-sdk.ps1`, `scripts/generate-mobile-sdk.sh`

**Rules:**

1. Generated output is committed once pipeline is active (M2 CI drift check)
2. Manual edits inside generated folders are forbidden
3. `core/api/` provides adapters (auth headers, base URL) around generated APIs
4. API contract PRs must regenerate SDK in the same PR

## Consequences

- M1 ships tooling and docs; first generated SDK in M2 when API stabilizes for mobile consumption
- No duplicate DTOs in `lib/features/`

## References

- [sdk-generation.md](../mobile/sdk-generation.md)
- [api-client.md](../mobile/foundation/api-client.md)

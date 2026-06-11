# Mobile testing strategy

## Pyramid

| Level | Tool | Scope |
|-------|------|-------|
| Unit | `flutter test` | Providers, repositories, mappers |
| Widget | `flutter test` | Screens, forms, error states |
| Integration | `integration_test` | Login flow, API smoke (QA env) |

---

## Coverage expectations

| Area | Minimum (M2+) |
|------|----------------|
| `core/auth` | 80% |
| `core/api` interceptors | 80% |
| Feature repositories | 70% |
| Widgets (critical paths) | Login, MFA, upload |

---

## Practices

1. **Override Riverpod** in tests — mock repositories, not Dio directly.
2. **Golden tests** for key screens (optional M3).
3. **No live API in unit tests** — use mocked SDK client.
4. Integration tests run against **QA** or docker-compose stack in CI.

---

## CI expectations (M1+)

| Job | Trigger |
|-----|---------|
| `flutter analyze` | Every PR |
| `flutter test` | Every PR |
| `integration_test` | Nightly or main branch |

Workflow file: `.github/workflows/mobile.yml` (created in M1, not M0).

---

## Parity with platform

Mobile CI follows same governance as backend `ci.yml` — docs validation + build + test.

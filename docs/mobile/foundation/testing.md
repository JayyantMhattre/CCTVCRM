# Testing (foundation)

Test layout mirrors `lib/`:

```
test/
├── core/
│   ├── correlation/
│   └── environment/
└── app/
    └── shell/
```

## Configured in M1

| Type | Examples |
|------|----------|
| Unit | `correlation_id_test.dart`, `environment_config_test.dart` |
| Widget | `splash_page_test.dart` |

## Commands

```bash
cd FrontEnd.Mobile
flutter test
flutter test test/core/correlation/correlation_id_test.dart
```

## Patterns

- **Pure logic:** test without Flutter binding
- **Providers:** `ProviderContainer` + `mocktail` overrides
- **Widgets:** `flutter_test` + `pumpWidget`
- **HTTP:** mock `Dio` adapter or `mocktail` interceptors (M2 feature tests)

## CI

`mobile.yml` runs `flutter test` on every PR touching `FrontEnd.Mobile/`.

## Future

- Integration tests with `integration_test/` (M2)
- Golden tests for shared widgets (M3)
- SDK contract tests against OpenAPI fixtures

See also [testing-strategy.md](../testing-strategy.md).

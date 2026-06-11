# Mobile coding standards

Applies when `FrontEnd.Mobile/` is created (M1+).

---

## Language & tooling

| Item | Standard |
|------|----------|
| Language | Dart 3.x, null-safe |
| Linter | `flutter_lints` / `very_good_analysis` (decide M1) |
| Formatter | `dart format` — enforced in CI |
| Imports | Package imports; feature-relative within feature |

---

## Structure

- One public widget per file for screens.
- `providers/`, `repositories/`, `presentation/` under each feature.
- File naming: `snake_case.dart`.

---

## State & UI

- Riverpod for business state ([state-management.md](./state-management.md)).
- No business logic in `build()` methods.
- Use `Theme.of(context)` / design tokens — no hard-coded brand colors in features.

---

## API & models

- **Use generated SDK only** for request/response types.
- Repositories return `Result` or throw mapped `ApiException` — mirror web error UX.

---

## Documentation

- Public classes: dartdoc `///` for non-obvious behavior.
- Feature README in `features/{name}/README.md` when feature ships.

---

## Git

- Conventional commits: `feat(mobile-auth): ...`, `fix(mobile-files): ...`
- No generated SDK manual edits.

See [mobile-governance.md](./mobile-governance.md).

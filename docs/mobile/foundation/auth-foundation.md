# Auth foundation

M1 provides **token infrastructure only** — no login, register, or MFA UI (M2).

## Components

| File | Role |
|------|------|
| `token_pair.dart` | Access + refresh token model |
| `token_storage.dart` | Persist/load tokens via `SecureStorage` |
| `auth_session.dart` | `AuthSessionNotifier` — Riverpod auth state |

## Secure storage

Tokens stored under keys defined in `token_storage.dart`. Implementation: `FlutterSecureStorageImpl`.

- **Android:** Encrypted SharedPreferences (via `flutter_secure_storage`)
- **iOS:** Keychain

See [ADR-Mobile-0004](../../adr/ADR-Mobile-0004-secure-token-storage.md).

## Session lifecycle (M1)

1. App start → `AuthSessionNotifier.build()` reads secure storage
2. Valid pair → `AuthSessionState.authenticated`
3. Missing/expired → `AuthSessionState.unauthenticated` → router sends user to `/unauthorized`
4. API 401 + successful refresh → session updated in place
5. Refresh failure → tokens cleared → unauthorized

## OAuth alignment

Same endpoints as web:

- Token: `POST /connect/token` (password grant in M2, refresh in M1 infra)
- API: `Authorization: Bearer {access_token}`

No mobile-specific auth APIs.

## M2 additions

- Login / register screens in `features/auth/`
- MFA challenge step
- Biometric unlock (M4)

## D1-13 Wave 4 — Password reset (native)

Public routes (no session required):

| Screen | Route | API |
|--------|-------|-----|
| Request OTP | `/forgot-password` | `POST /auth/password-reset/request` |
| Verify OTP + reset | `/reset-password` | `POST /auth/password-reset/verify`, `/confirm` |

Entry: `UnauthorizedPage` → Forgot password link.

- **Email OTP:** platform email template (`password-reset-otp`)
- **Mobile OTP:** optional phone on request screen; API accepts `phoneNumber` — SMS delivery is a **V1.1 candidate** (Auth email-only today)
- **Reset password:** challenge + new password on `/reset-password`

Files: `lib/features/auth/pages/forgot_password_page.dart`, `reset_password_page.dart`, `data/password_reset_repository.dart`.

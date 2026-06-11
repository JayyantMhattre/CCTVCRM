# Biometric authentication (M4)

**Path:** `lib/core/auth/biometrics/`

## Contract

`BiometricService` — `local_auth` adapter (`LocalAuthBiometricService`).

## Security

- **No credential storage** — only gates access to tokens already in secure storage.
- Fallback: password sign-in via `/unauthorized`.

## UX

- Enable/disable on Profile page.
- `/biometric-gate` when enabled and session exists.
- Android: fingerprint, face unlock. iOS: Face ID, Touch ID.

Feature flag: `mobile.biometrics`

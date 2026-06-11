# ADR-Mobile-0004: Secure token storage

## Status

Accepted (M1)

## Context

Access and refresh tokens must persist across app restarts using platform-native secure storage on Android and iOS.

## Options evaluated

| Option | Pros | Cons |
|--------|------|------|
| **`flutter_secure_storage`** | Keychain + EncryptedSharedPreferences, widely used | Plugin maintenance dependency |
| **Shared preferences (plain)** | Simple | Unacceptable for refresh tokens |
| **Custom platform channels** | Full control | High maintenance |

## Decision

Abstract storage behind `SecureStorage` interface; default implementation `FlutterSecureStorageImpl` using **`flutter_secure_storage`**.

Tokens accessed only through `TokenStorage` in `core/auth/`.

## Consequences

- Features never read/write token keys directly
- Biometric gate (M4) wraps `TokenStorage` read path — no storage swap required
- Unit tests inject in-memory `SecureStorage` fake

## References

- [auth-foundation.md](../mobile/foundation/auth-foundation.md)
- [security.md](../mobile/security.md)

# MFA setup

1. Authenticated `POST /auth/mfa/enroll` → `{ secret, authenticatorUri }`
2. Scan URI in authenticator app
3. `POST /auth/mfa/confirm` with `{ secret, code }`

Disable: `POST /auth/mfa/disable` with current TOTP code.

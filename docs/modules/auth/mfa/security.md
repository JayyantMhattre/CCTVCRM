# MFA security

- Secrets stored on `auth.users.mfa_secret` (enable only after confirm).
- Challenge IDs are opaque GUIDs in Redis, not JWTs.
- No backup codes in this phase (optional future).

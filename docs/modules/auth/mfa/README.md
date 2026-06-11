# MFA (TOTP)

TOTP-based MFA using **Otp.NET**.

## Endpoints

| Method | Path |
|--------|------|
| POST | `/api/v1/auth/mfa/enroll` |
| POST | `/api/v1/auth/mfa/confirm` |
| POST | `/api/v1/auth/mfa/disable` |
| POST | `/api/v1/auth/mfa/verify` |

## Login

1. `POST /connect/token` (password grant)
2. If `mfa_required`, response includes `mfa_challenge_id`
3. `POST /connect/token` with `grant_type=mfa`, `mfa_challenge_id`, `mfa_code`

Tenant `RequireMfa` forces challenge even when user has not enrolled (must enroll first).

See [setup.md](./setup.md), [flows.md](./flows.md).

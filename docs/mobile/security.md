# Mobile security

Mobile **reuses backend auth** — no separate identity design.

---

## Authentication

| Mechanism | Backend | Mobile (planned) |
|-----------|---------|------------------|
| Login | `POST /connect/token` (password grant) | Same |
| MFA | `grant_type=mfa` + `mfa_challenge_id` | Challenge screen after login |
| Refresh | Refresh token (cookie or body per host config) | Secure storage + interceptor |
| Register | `POST /api/v1/auth/register` | Same payload as web |
| Logout | Revoke sessions API + clear local tokens | `POST /auth/sessions/revoke-all` optional |

---

## Token storage

| Data | Storage |
|------|---------|
| Access token | `flutter_secure_storage` |
| Refresh token | `flutter_secure_storage` |
| User claims (decoded JWT) | Memory + optional prefs (non-sensitive) |

Never store tokens in plain `SharedPreferences`.

---

## Tenant context

- Resolve `tenantId` from JWT (same claims as web).
- Send tenant on API calls per host middleware expectations.
- Switching tenant (if multi-tenant UX added) requires re-auth or token with new tenant — follow backend rules.

---

## Session management

- List/revoke via `/api/v1/auth/sessions` (parity with web Sessions page).
- Local session = valid tokens + optional device registration for push (future).

---

## Transport

- **Prod:** HTTPS only; certificate pinning evaluated in M3 ADR if required.
- **Dev:** cleartext localhost allowed via Android network security config (dev flavor only).

---

## Files

- Upload through authenticated API only — no direct blob credentials on device.
- Download via authenticated stream; cache encrypted at rest if cached (M4).

---

## Biometrics (future)

Optional app unlock using biometrics — does not replace server MFA.

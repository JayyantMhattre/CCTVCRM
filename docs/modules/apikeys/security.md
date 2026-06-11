# API Keys security

## Secret handling

- Cryptographically secure random generation (`RandomNumberGenerator`)
- **Argon2id** hash stored in database
- Plaintext shown **once** on create/rotate
- Never logged, never returned on GET

## Key format

```
ashk_{environment}_{prefixId}_{secret}
```

Environments: `dev`, `qa`, `uat`, `prod` (configured via `ApiKeys:Environment`).

## Tenant isolation

- Every key belongs to exactly one `tenant_id`
- EF global query filter on tenant context
- Validator loads by prefix without cross-tenant leakage
- API key principal carries single tenant claim

## Revocation

Immediate effect — `RevokedOnUtc` set, `Enabled = false`, validator rejects.

## Rate limiting

- Per-key subject in Redis rate limit cache when `api_key_id` claim present
- Stricter limit on key creation endpoint (`apikeys/create` route policy)

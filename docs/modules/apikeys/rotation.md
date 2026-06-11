# API key rotation & revocation

## Rotation

`POST /api/v1/api-keys/{id}/rotate`

- Generates new `ashk_{env}_*` secret
- Replaces `hashed_secret` and `key_prefix`
- Old secret immediately invalid
- Returns `ApiKeyCreatedContract` with plaintext (once)
- Raises `ApiKeyRotatedDomainEvent` → audit

Requires `apikeys:manage`.

## Revocation

`POST /api/v1/api-keys/{id}/revoke`

- Sets `revoked_on_utc`, disables key
- Validator rejects on next request
- Raises `ApiKeyRevokedDomainEvent` → audit

Requires `apikeys:manage`.

## Scope changes

`PUT /api/v1/api-keys/{id}/scopes`

- Updates scope array
- Raises `ApiKeyScopesChangedDomainEvent` → audit

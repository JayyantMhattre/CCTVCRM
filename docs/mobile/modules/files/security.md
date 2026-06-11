# Files — security

1. **Tenant scope** — backend enforces tenant + user on all file operations.
2. **No direct blob access** — mobile never talks to S3/Azure; only Ashraak API.
3. **Auth-gated streams** — download/preview use Bearer token via Dio interceptors.
4. **URL endpoint** — `GET /files/{id}/url` returns API path, not a public presigned URL.
5. **Delete** — soft-delete on server; confirm dialog on mobile before `DELETE`.

Correlation IDs on all requests for support traceability.

# Error Catalog

## OAuth token endpoint (`POST /connect/token`)

JSON body — not ProblemDetails.

| error | error_description (typical) | HTTP |
|-------|----------------------------|------|
| `invalid_request` | Form-encoded body required | 400 |
| `unsupported_grant_type` | Only password grant supported | 400 |
| `invalid_tenant` | Valid tenant_id required | 400 |
| `tenant_inactive` | Tenant inactive | 400 |
| `invalid_grant` | Invalid credentials or locked user | 400 |

---

## REST API (ProblemDetails)

Global handler: `GlobalExceptionHandler` → RFC 7807 for unhandled exceptions (500).

Command failures often return:

```http
400 Bad Request
Content-Type: application/problem+json
```

Body includes `detail` from `Result.Error.Description`.

| HTTP | Meaning |
|------|---------|
| 400 | Validation / business rule failure |
| 401 | Missing or invalid Bearer token |
| 403 | Forbidden — tenant scope or policy |
| 404 | Entity not found |
| 500 | Unhandled exception |

---

## Auth module permissions

Permission strings (JWT `permission` claim):

| Permission | Description |
|------------|-------------|
| `audit:read` | Read audit logs |
| `users:manage` | Manage users |
| `user:invite` | Invite users |
| `tenant:configure` | Tenant settings |

Defined in `Auth.Domain/ValueObjects/Permission.cs`.

---

## Tenant context errors

| Message | Endpoint | Cause |
|---------|----------|-------|
| No tenant context was resolved | `/tenants/current`, `/users/tenant/current` | Missing JWT tenant claim |

---

## Audit API

Stub — no error catalog beyond standard 401/403 for non-Admin.

---

## Related

- [problem-details.md](./problem-details.md)
- [common-failures.md](./common-failures.md)

# Files — security

## Tenant scoping

Mandatory on all operations. Cross-tenant file access returns 404.

## Download access

No anonymous URLs. `GetUrlAsync` returns `/api/v1/files/{id}` path for authenticated clients.

## Upload validation

- Configurable `MaxUploadBytes`
- `AllowedContentTypes` allow-list (empty = allow all)

## Virus scan hook

`IFileScanService` — stub returns `Clean`. Replace in Infrastructure for ClamAV or cloud AV.

## Audit

Upload, download, delete raise domain events → existing audit pipeline.

# Files — architecture

## Projects

| Project | Responsibility |
|---------|----------------|
| `Ashraak.Files.Domain` | `FileRecord` aggregate, domain events |
| `Ashraak.Files.Application` | Upload / download / delete commands |
| `Ashraak.Files.Infrastructure` | EF, providers, `IFileStorage` impl |
| `Ashraak.Files.Api` | Minimal API endpoints |

## Data split

| Store | Content |
|-------|---------|
| PostgreSQL `files` schema | `FileRecord` metadata |
| Provider backend | Blob bytes |

## Tenant isolation

- EF global query filter on `TenantId`
- Storage path prefix: `{tenantId}/files/{fileId}/`
- Download/delete verify tenant + soft-delete state

## Audit

Domain events (`FileUploaded`, `FileDownloaded`, `FileDeleted`) flow through outbox → MediatR → `DomainEventAuditHandler`.

## Registration

`AddFilesModule()` in `FilesModule.cs` — wired from `ModuleExtensions.cs` after Users.

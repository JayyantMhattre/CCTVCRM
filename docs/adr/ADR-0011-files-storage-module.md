# ADR-0011: Files and storage provider abstraction

## Status

Accepted

## Context

Platform needs reusable tenant-scoped file storage for future modules (avatars, exports, attachments).

## Decision

- New **Files** module (Domain / Application / Infrastructure / Api).
- Contract **`IFileStorage`** in `SharedKernel.Contracts.Storage`.
- Metadata in PostgreSQL (`files` schema); blobs via provider pattern.
- Providers: **Local** (dev default), **S3-compatible**, **Azure Blob** — HTTP-based, no cloud SDK lock-in.
- **`IFileScanService`** stub for future AV.
- React shared **`file-upload`** infrastructure component (no feature pages).
- Storage health check on `/health/ready`.

## Consequences

- Host registers `AddFilesModule`, outbox processor, health check.
- Storage paths: `{tenantId}/files/{fileId}/`.
- Downloads are auth-gated API streams, not public URLs.

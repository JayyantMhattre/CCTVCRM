# Files module

Platform storage capability — tenant-scoped file metadata (PostgreSQL) and blobs (local disk or cloud provider).

## Quick links

| Document | Purpose |
|----------|---------|
| [architecture.md](./architecture.md) | Layers, boundaries, data flow |
| [storage-providers.md](./storage-providers.md) | Local, S3-compatible, Azure Blob |
| [api.md](./api.md) | HTTP endpoints |
| [security.md](./security.md) | Tenant isolation, scan hook, access |
| [operations.md](./operations.md) | Config, health, troubleshooting |
| [extending.md](./extending.md) | New providers, cross-module usage |

## Contract

Other modules use **`IFileStorage`** from `SharedKernel.Contracts.Storage` — never reference Files.Infrastructure directly.

## Default (dev)

`Storage:Provider` = `Local` — files under `Storage:Local:RootPath` (default `./data/files`).

# Files — operations

## Health

`/health/ready` includes `storage` check (provider registered + local path writable or cloud HEAD).

## Connection string

```json
"ConnectionStrings": {
  "Files": "Host=...;Search Path=files"
}
```

## Bootstrap

`BackEnd/scripts/init-db.sql` creates `files` schema and `file_records` table.

## Disk growth (local)

Monitor `Storage:Local:RootPath`. Rotate or switch to `S3`/`Azure` for production.

## Outbox

`OutboxProcessorHostedService<FilesDbContext>` registered in host — domain events audited asynchronously.

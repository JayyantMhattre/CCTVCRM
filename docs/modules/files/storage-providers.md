# Storage providers

Config section: `Storage`

| Provider | Value | Use case |
|----------|-------|----------|
| Local | `Local` | Dev default — disk under `Local:RootPath` |
| S3-compatible | `S3` | MinIO, AWS S3, Cloudflare R2 (HTTP + bearer/api-key) |
| Azure Blob | `Azure` | Azure Blob REST via `Azure:BlobEndpoint` + SAS/token |

No cloud SDK dependencies — providers use `HttpClient` with configurable auth headers.

## Local

```json
"Storage": {
  "Provider": "Local",
  "Local": { "RootPath": "./data/files" },
  "MaxUploadBytes": 10485760,
  "AllowedContentTypes": [ "image/png", "image/jpeg", "application/pdf" ]
}
```

## S3-compatible

```json
"S3": {
  "ServiceUrl": "https://minio.example.com",
  "Bucket": "ashraak",
  "AuthHeaderName": "Authorization",
  "AuthHeaderValue": "Bearer <token-or-proxy-key>"
}
```

## Azure Blob

```json
"Azure": {
  "BlobEndpoint": "https://account.blob.core.windows.net",
  "Container": "ashraak",
  "AuthHeaderName": "x-ms-blob-type",
  "AuthHeaderValue": "BlockBlob"
}
```

Production: place API gateway or SAS token in `AuthHeaderValue` per your security model.

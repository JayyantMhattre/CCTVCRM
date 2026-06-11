# Files — API

Base: `/api/v1/files` (versioned host group)

All endpoints require authentication.

## POST /api/v1/files

Multipart upload (`file` field).

**Validation:** max size, allowed content types (config).

**Response 201:**

```json
{
  "id": "guid",
  "fileName": "doc.pdf",
  "contentType": "application/pdf",
  "size": 12345,
  "uploadedOnUtc": "2026-05-31T12:00:00Z"
}
```

## GET /api/v1/files/{fileId}

Streams file content. `Content-Type` from metadata. Auth + tenant scoped — not a public URL.

## DELETE /api/v1/files/{fileId}

Soft delete. Returns 204.

## GET /api/v1/files/{fileId}/url

Returns authenticated API download path (not anonymous presigned URL).

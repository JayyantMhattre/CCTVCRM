# Files — download & preview

## Download

`FilesRepository.download` uses authenticated `GET /files/{fileId}` with `ResponseType.bytes`.

Saved to temp directory on device; user notified via success toast.

## Preview

| Type | Behavior |
|------|----------|
| Image | `AuthenticatedImage` — Dio download with Bearer token |
| PDF | Download to temp → `OpenFilex.open` |
| Other | Metadata card + manual download |

## Navigation

- List: `/files`
- Preview: `/files/{fileId}/preview`

Metadata must exist in `FilesProvider` session list (upload or open from list).

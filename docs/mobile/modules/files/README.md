# Mobile — Files module

**Feature:** `FrontEnd.Mobile/lib/features/files/`

Reuses backend Files APIs — no mobile-specific endpoints.

## APIs

| Action | Method | Path |
|--------|--------|------|
| Upload | `POST` | `/api/v1/files` (multipart field `file`) |
| Download | `GET` | `/api/v1/files/{fileId}` |
| URL hint | `GET` | `/api/v1/files/{fileId}/url` |
| Delete | `DELETE` | `/api/v1/files/{fileId}` |

## Mobile UX

- Upload: camera, gallery, document picker (`FileSourceProvider`)
- Session file list (no backend list API — uploads tracked in `FilesProvider`)
- Preview: images inline; PDF via download + `open_filex`
- Errors: `ApiError` + `AppToast` + correlation banner

## Docs

- [upload-flow.md](./upload-flow.md)
- [download-flow.md](./download-flow.md)
- [security.md](./security.md)

## Web reference

`FrontEnd/apps/web/src/shared/file-upload/`

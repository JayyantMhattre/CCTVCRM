# Files — upload flow

```mermaid
sequenceDiagram
    participant UI as FilesPage
    participant Picker as FileSourceProvider
    participant Repo as FilesRepository
    participant API as POST /api/v1/files

    UI->>Picker: camera / gallery / document
    Picker-->>UI: PickedFileSource
    UI->>Repo: multipart upload + progress
    Repo->>API: Bearer + X-Correlation-Id
    API-->>Repo: UploadFileResult
    Repo-->>UI: UploadedFile in FilesProvider
```

## Sources

| Source | Implementation |
|--------|----------------|
| Camera | `image_picker` `ImageSource.camera` |
| Gallery | `image_picker` `ImageSource.gallery` |
| Documents | `file_picker` (PDF, Office, text) |

## Types

Images, PDFs, and generic documents via MIME detection. Extensible in `MobileFileSourceProvider._guessMimeType`.

## Errors

Upload failures surface via `AppToast.error` with `ApiError` message and correlation ID when present.

# File upload (shared infrastructure)

Reusable upload control for feature modules — not a standalone page.

**Location:** `FrontEnd/apps/web/src/shared/file-upload/`

## Usage

```tsx
import { FileUpload } from '@/shared/file-upload';

<FileUpload
  accept="image/png,image/jpeg"
  maxBytes={5_000_000}
  onUploaded={(file) => console.log(file.id)}
/>
```

## Behaviour

- Multipart POST to `/api/v1/files`
- Progress bar during upload
- Toast on success / error (via `useToast` + `useApiError`)

## Related

- [Files module API](../../modules/files/api.md)

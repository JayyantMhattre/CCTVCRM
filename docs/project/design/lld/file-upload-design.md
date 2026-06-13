# File Upload Design

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Mandate:** REUSE platform Files module — [file-upload README](../../../frontend/file-upload/README.md) · [Files API](../../../modules/files/api.md)

Two-step pattern everywhere: **Upload to `/api/v1/files`** → **Link via CCTV attachment endpoint**.

---

## 1. Web component (REUSE)

```tsx
<FileUpload
  accept={acceptMap[category]}
  maxBytes={sizeLimit[category]}
  onUploaded={(file) => setValue('fileId', file.id)}
/>
```

- Progress bar during upload (platform)
- Toast on success/error via `useToast` + `useApiError`
- Permission: JWT with `files:write` + business permission on link step

---

## 2. Upload categories

| Category | Screen(s) | accept | maxBytes | max count | Link API |
|----------|-----------|--------|----------|-----------|----------|
| Lead attachment | #13 | `.pdf,.doc,.docx,.png,.jpg,.jpeg` | 10 MB | 10 | `POST .../leads/{id}/attachments` |
| Site document | #18 | `.pdf,.png,.jpg,.jpeg` | 10 MB | 20 | `POST .../sites/{id}/documents` |
| Contract PDF (upload) | #24 | `.pdf` | 15 MB | 5 | `POST .../contracts/{id}/documents` |
| Visit photo Before/During/After | #62–63 | `image/jpeg,image/png,image/webp` | 10 MB | 20 per visit | `POST .../visits/{id}/photos` |
| Visit selfie | #64 | `image/jpeg,image/png` | 5 MB | 1 | `POST .../selfie` |
| Visit video | #63 | `video/mp4,video/quicktime` | 100 MB | 3 | `POST .../attachments` |
| Visit signature | #66 | `image/png` | 1 MB | 1 | `POST .../signature` |
| Ticket attachment | #32, #52 | `image/*,.pdf` | 10 MB | 5 per ticket | `POST .../tickets/{id}/attachments` |
| Invoice PDF | system | — | — | — | Generated server-side → Files |

---

## 3. Validation (client + server)

| Rule | Enforcement |
|------|-------------|
| File size | `FileUpload maxBytes` + platform `MaxUploadBytes` |
| Content type | `accept` prop + platform allow-list + CCTV link validator |
| Empty file | Reject 0-byte |
| Virus scan | Platform stub → production AV hook |
| Tenant scope | Platform Files — automatic |
| Business ownership | CCTV link API verifies role + entity scope before accepting `fileId` |

---

## 4. Download / preview

| Context | Pattern |
|---------|---------|
| Admin evidence review | Thumbnail grid → click opens new tab `GET /files/{id}` |
| Customer report PDF | Authorized link → download |
| Invoice PDF | `GET /invoices/{id}/pdf` authorizes then streams FileId |
| Mobile preview | REUSE `features/files` preview + OpenFilex for PDF |

**Never** expose raw storage URLs — authenticated API paths only.

---

## 5. Mobile upload (REUSE `features/files`)

| Source | Usage |
|--------|--------|
| Camera | Selfie, visit photos |
| Gallery | Photos, ticket attachments |
| File picker | Documents |
| Signature canvas | Export PNG → upload as file |

**Offline (Engineer):** Queue bytes locally → upload on sync → then link in batch ([mobile-api-consumption.md](../mobile-api-consumption.md))

---

## 6. Delete behavior

| Action | Effect |
|--------|--------|
| Remove attachment link | DELETE CCTV attachment row |
| Platform file delete | `DELETE /files/{id}` when no other references (admin only) |
| Generated PDFs | Immutable after Generate — no delete V1 |

Confirm dialog before delete — REUSE `PlatformConfirmDialog`.

---

## 7. UI placement patterns

| Pattern | Usage |
|---------|--------|
| Inline single | Selfie, signature |
| Multi-file list | Ticket attachments, lead attachments |
| Category tabs | Visit photos (Before/During/After) |
| Drag-drop zone | Admin document uploads (FileUpload supports) |
| Read-only gallery | Visit review #29 |

---

## 8. Error UX

| Error | Message |
|-------|---------|
| Size exceeded | "File exceeds {limit} MB limit." |
| Type rejected | "File type not allowed." |
| Upload failed | Toast + correlation id |
| Link failed | "Upload succeeded but could not attach. Retry link." |

---

## 9. Classification

| Item | Class |
|------|:-----:|
| Files API | REUSE |
| FileUpload component | REUSE |
| Mobile files feature | REUSE |
| CCTV link endpoints | NEW |
| Category validation config | NEW |
| PDF generation → Files | NEW service + REUSE storage |

---

Related: [form-catalog.md](./form-catalog.md) · [pdf-document-design.md](./pdf-document-design.md) · [file-management-design.md](../file-management-design.md)

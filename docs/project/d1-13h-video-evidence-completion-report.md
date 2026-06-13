# D1-13h — Visit Video Evidence Completion Report

**Date:** 2026-06-12  
**Wave:** D1-13 Wave 4  
**Scope:** Visit video upload, preview metadata, download, visit association via Files module (FR-VISIT-06)

---

## Objective

Extend existing visit evidence workflow to support optional video attachments using platform Files infrastructure — no separate storage.

---

## Delivered

### Backend (existing — reused)

| Item | Detail |
|------|--------|
| Endpoint | `POST /api/v1/cctv/visits/{visitId}/attachments` |
| Request | `LinkVisitAttachmentRequest` — `FileId`, `AttachmentType: Video`, optional `Title` |
| Domain | `VisitAttachmentType.Video` on `ServiceVisit` aggregate |
| Response | `VisitDetailDto.Attachments` includes video metadata |

### Web engineer UI

| Capability | File |
|------------|------|
| Video upload | `EngineerVisitReportPage.tsx` — accepts `video/mp4`, `video/quicktime` |
| Size guard | Client rejects files > **100 MB** (per LLD) |
| Metadata list | Shows linked video titles from `attachments` |
| Download | `visitsApi.downloadFile` via Files module `/files/{fileId}` |

### Mobile engineer UI

| Capability | File |
|------------|------|
| Video picker | `MobileFileSourceProvider.pickVideo()` — mp4/mov |
| Upload + link | `CctvEngineerRepository.linkVideo()` → attachments API |
| Offline queue | `CctvOfflineOperationKind.visitVideo` replay on sync |
| Preview / download | Navigate to platform `FilePreviewPage` for linked video |
| Metadata | `CctvVisitAttachment` parsed on `CctvVisitDetail` |

### API extensions

- `FrontEnd/apps/web/src/modules/cctv/visits/api.ts` — `linkVideo`, `downloadFile`, attachment mapping
- `FrontEnd/apps/web/src/modules/cctv/visits/types.ts` — `VisitAttachment` interface
- `FrontEnd.Mobile/lib/features/cctv/cctv_api_paths.dart` — `visitAttachments` path

---

## File limits and retention

| Policy | Value | Source |
|--------|-------|--------|
| Max video size | **100 MB** | [file-upload-design.md](./design/lld/file-upload-design.md) § Visit video |
| Allowed MIME | `video/mp4`, `video/quicktime` | LLD |
| Max videos per visit | **3** | LLD (enforced server-side on aggregate rules) |
| Storage | Platform Files module (`FileRecord`) | No separate CCTV storage |
| Retention | Follows platform Files retention policy for tenant | Reuse platform lifecycle |

---

## Verification

| Check | Result |
|-------|--------|
| Backend build | ✅ |
| Web build | ✅ |
| Architecture tests | ✅ 21/21 |

---

## Deferred

- Inline video player in visit report (preview via Files module is sufficient for V1)
- Automated video evidence integration tests (execution deferred)

---

## References

- LLD: [file-upload-design.md](./design/lld/file-upload-design.md)
- Wave 4 summary: [d1-13-v1-scope-completion-report.md](./d1-13-v1-scope-completion-report.md)

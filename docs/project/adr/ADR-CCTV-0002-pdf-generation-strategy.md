# ADR-CCTV-0002 — PDF Generation Strategy (Sprint 0)

**Status:** Accepted (stub phase)  
**Date:** Sprint 0  
**Context:** Freeze §19 requires Contract, Visit Report, and Invoice PDFs. Platform has no PDF service.

## Decision

1. Introduce `IPdfGenerationService` in CCTV Integration Application layer.
2. Sprint 0 ships `StubPdfGenerationService` (minimal bytes — not a valid PDF for production).
3. Generated PDFs will be stored via **platform Files module** (two-step upload) — no blob paths in business tables.
4. Library selection deferred to **NR-02** — evaluate QuestPDF, iText 7 (AGPL/commercial), or HTML-to-PDF for V1.

## Consequences

- B3/B4/B6 implement real templates against the interface.
- No duplicate file storage.
- Library ADR amendment when implementation begins in B3.

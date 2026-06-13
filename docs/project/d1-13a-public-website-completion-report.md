# Sprint 1 — D1-13a Completion Report

**Phase:** D1-13a Public Website  
**Date:** 2026-06-11  
**Wave:** D1-13 Wave 1 (stop after Wave 1)  
**Review gate:** Deferred test execution (restore + build + architecture tests)

## Summary

Delivered the anonymous public website module inside the existing React SPA (not a separate application). All required routes, showcase pages, inquiry forms, and basic SEO are wired to the existing `POST /api/v1/cctv/inquiries` endpoint.

## Requirements closed

| ID | Status |
|----|--------|
| FR-WEB-01 | **Closed** — Home, About, Services, AMC, Contact, Gallery, Testimonials pages |
| FR-WEB-02 | **Closed** — Login entry links to platform `/login` |
| FR-WEB-03 | **Closed** — Get Quote form → `InquiryType=GetQuote` |
| FR-WEB-04 | **Closed** — AMC Inquiry form → `InquiryType=AmcInquiry` |
| FR-WEB-05 | **Closed** (pre-existing) — Inquiry API creates leads |
| BO-1 (partial) | **Closed** — Public lead capture UI |

## Public routes

| Route | Page |
|-------|------|
| `/` | Home |
| `/about` | About Us |
| `/services` | Services |
| `/amc` | AMC Plans |
| `/contact` | Contact Us (+ Contact inquiry form) |
| `/gallery` | Gallery |
| `/testimonials` | Testimonials |
| `/login` | Platform auth login (existing) |
| `/get-quote` | Get Quote form |
| `/amc-inquiry` | AMC Inquiry form |

## Frontend deliverables

| Area | Path |
|------|------|
| Route tree | `FrontEnd/apps/web/src/modules/cctv/public/routes.tsx` |
| Layout + SEO | `PublicLayout.tsx`, `PublicSeo.tsx` |
| Content | `content.ts` (Aarvii messaging reuse) |
| Inquiry client | `inquiriesApi.ts` → `POST /cctv/inquiries` |
| Router integration | `core/router/index.tsx` — public routes precede auth; `/` no longer redirects to dashboard |

## SEO (basic)

- `document.title` per route via `PublicSeo`
- Meta description
- OpenGraph title, description, type, url

## Backend changes

None — reuses existing inquiry API and lead pipeline.

## Verification

```bash
dotnet build BackEnd/src/Host/Ashraak.Api
dotnet test BackEnd/tests/Ashraak.Architecture.Tests   # 20/20 PASS
```

## Deferred

- Advanced SEO (sitemap, structured data, analytics)
- Live www.aarvii.in content import (static copy aligned to freeze scope)
- E2E public form tests (deferred to release gate)

## References

- [requirements-freeze-v1.md](./requirements-freeze-v1.md) §2 Public Website
- [navigation-map.md](./navigation-map.md) §1

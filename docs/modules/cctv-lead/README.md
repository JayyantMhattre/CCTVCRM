# Lead Management

**Status:** B1 complete — Lead module + inquiry API + admin UI  
**Schema:** `cctv_lead`  
**Implementation phase:** B1 (Sprint 1)

Aarvii CCTV AMC business module on Ashraak Platform V1.

## Scope (B1)

- Domain aggregate: Lead, LeadActivity, LeadRemark, LeadAttachment
- Public inquiry API (`POST /api/v1/cctv/inquiries`)
- Admin lead CRUD, status transitions, activities, remarks, attachments
- Lead conversion orchestration (stub downstream until B2/B3)
- Admin lead pipeline UI (list + detail)

## Projects

See `BackEnd/src/Modules/Cctv/Lead/` for source layout.

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Feature flag

Enable with `cctv.leads.enabled` (backend `Features:Flags` and frontend `CCTV_FEATURE_FLAG_DEFAULTS`).

## Migration

```bash
dotnet ef database update \
  --project BackEnd/src/Modules/Cctv/Lead/Ashraak.Cctv.Lead.Infrastructure \
  --context LeadDbContext
```

Design-time factory: `LeadDbContextFactory` (no host/Redis required).

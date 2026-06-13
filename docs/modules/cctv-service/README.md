# Scheduling & Visits

**Status:** D1-5 complete (Review Gate 1)  
**Schema:** `cctv_service`  
**Implementation phase:** D1-5 / B4

Service Scheduling and Visit Management for Aarvii CCTV AMC — schedules, engineer assignment, visit evidence, and admin approval.

## Projects

```
BackEnd/src/Modules/Cctv/Service/
  Ashraak.Cctv.Service.Domain/
  Ashraak.Cctv.Service.Application/
  Ashraak.Cctv.Service.Infrastructure/
  Ashraak.Cctv.Service.Api/
```

## Deliverables (D1-5)

- [x] `ServiceSchedule` aggregate with engineer assignment history
- [x] `ServiceVisit` aggregate with evidence entities (photos, GPS, signature, attachments, approvals)
- [x] Auto-schedule generation on AMC term activation (`TermActivatedScheduleGenerationHandler`)
- [x] REST endpoints under `/api/v1/cctv/schedules`, `/visits`, `/portal/visits`, `/engineer/*`
- [x] SharedKernel DTOs and lookup/generation contracts
- [x] Admin UI: schedules list, visits list + detail
- [x] Engineer UI: assigned schedules (`/engineer/visits`)
- [x] Domain tests (`ServiceDomainTests.cs`) — created, execution deferred to Review Gate 2

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Related

- Design: [erd-service-domain.md](../../project/design/erd-service-domain.md)
- Completion report: [sprint-1-d1-5-completion-report.md](../../project/sprint-1-d1-5-completion-report.md)

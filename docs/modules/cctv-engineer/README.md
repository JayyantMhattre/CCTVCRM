# Engineer module

**Status:** D1-5 stub — master data for assignment validation only  
**Schema:** `cctv_engineer`  
**Full implementation:** D1-8 (Engineer Management) — CRUD, workload, admin UI

## D1-5 scope

Minimal engineer aggregate and `IEngineerLookupService` (`ExistsAsync`, `GetAsync`, `GetForPlatformUserAsync`) used by Service module when assigning engineers to schedules.

No admin CRUD HTTP endpoints in D1-5 — engineer records are seeded or inserted via migration/dev scripts until B5.

## Projects

```
BackEnd/src/Modules/Cctv/Engineer/
  Ashraak.Cctv.Engineer.Domain/
  Ashraak.Cctv.Engineer.Infrastructure/  — EF + EngineerLookupService
```

Application and API layers remain skeleton placeholders for B5.

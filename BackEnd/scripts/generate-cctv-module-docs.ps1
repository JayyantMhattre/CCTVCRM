$modules = @(
    @{ Slug = 'cctv-lead'; Name = 'Lead Management'; Schema = 'cctv_lead'; Phase = 'B1' },
    @{ Slug = 'cctv-customer'; Name = 'Customer / Site / Asset'; Schema = 'cctv_customer'; Phase = 'B2' },
    @{ Slug = 'cctv-amc'; Name = 'AMC Plans & Contracts'; Schema = 'cctv_amc'; Phase = 'B3' },
    @{ Slug = 'cctv-service'; Name = 'Scheduling & Visits'; Schema = 'cctv_service'; Phase = 'B4' },
    @{ Slug = 'cctv-ticket'; Name = 'Ticket Management'; Schema = 'cctv_ticket'; Phase = 'B5' },
    @{ Slug = 'cctv-engineer'; Name = 'Engineer Management'; Schema = 'cctv_engineer'; Phase = 'B5' },
    @{ Slug = 'cctv-invoice'; Name = 'Invoice Management'; Schema = 'cctv_invoice'; Phase = 'B6' },
    @{ Slug = 'cctv-reporting'; Name = 'Reporting'; Schema = '(read-only)'; Phase = 'B7' },
    @{ Slug = 'cctv-integration'; Name = 'CCTV Integration'; Schema = 'N/A'; Phase = 'D1/Sprint 0' }
)

$docsRoot = Join-Path $PSScriptRoot '../../docs/modules'

foreach ($m in $modules) {
    $dir = Join-Path $docsRoot $m.Slug
    New-Item -ItemType Directory -Force -Path $dir | Out-Null

    @"
# $($m.Name)

**Status:** Sprint 0 skeleton — no business logic  
**Schema:** $($m.Schema)  
**Implementation phase:** $($m.Phase)

Aarvii CCTV AMC business module on Ashraak Platform V1.

## Projects

See ``BackEnd/src/Modules/Cctv/`` for source layout.

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)
"@ | Set-Content (Join-Path $dir 'README.md') -Encoding UTF8

    @"
# Architecture — $($m.Name)

Sprint 0: module skeleton registered at Host Layer 2. Domain entities arrive in phase $($m.Phase).

Design authority: ``docs/project/design/entity-model.md``
"@ | Set-Content (Join-Path $dir 'architecture.md') -Encoding UTF8

    @"
# Registration — $($m.Name)

Registered via ``AddCctvModules`` in ``ModuleExtensions.cs`` (Layer 2, after ApiKeys).
"@ | Set-Content (Join-Path $dir 'registration.md') -Encoding UTF8

    @"
# API — $($m.Name)

Sprint 0: route group placeholder under ``/api/v1/cctv``. Business endpoints per ``docs/project/design/endpoint-catalog.md`` in phase $($m.Phase).

| Method | Route | Status |
|--------|-------|--------|
| GET | ``/api/v1/cctv/health`` | Implemented (aggregate) |
"@ | Set-Content (Join-Path $dir 'api.md') -Encoding UTF8

    @"
# Events — $($m.Name)

Sprint 0: no domain events published. See ``docs/project/design/event-catalog.md`` for planned events.
"@ | Set-Content (Join-Path $dir 'events.md') -Encoding UTF8

    @"
# Extending — $($m.Name)

Follow ``docs/extending/add-backend-module.md``. Cross-module access via ``SharedKernel.Contracts.CctvCrm`` only.
"@ | Set-Content (Join-Path $dir 'extending.md') -Encoding UTF8

    @"
# Operations — $($m.Name)

**Health:** ``GET /api/v1/cctv/health`` (anonymous)  
**Schema:** $($m.Schema)
"@ | Set-Content (Join-Path $dir 'operations.md') -Encoding UTF8
}

Write-Host 'Module docs generated.'

# CCTV Integration

**Status:** D1-13 Wave 2 — notifications + PDF production  
**Schema:** N/A (cross-cutting adapters)  
**Implementation phase:** D1/Sprint 1

Aarvii CCTV AMC integration layer on Ashraak Platform V1 — SMS, PDF, notification dispatch, RBAC seed, reporting data provider.

## Projects

| Project | Role |
|---------|------|
| `Ashraak.Cctv.Integration.Application` | `ICctvNotificationDispatcher`, `ICctvFileStore`, template keys, SMS options |
| `Ashraak.Cctv.Integration.Infrastructure` | Handlers, QuestPDF, configured SMS, outbox processors, hosted jobs |

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Completion reports

- [D1-13c Notifications](../../project/d1-13c-notifications-completion-report.md)
- [D1-13d PDF Generation](../../project/d1-13d-pdf-generation-completion-report.md)

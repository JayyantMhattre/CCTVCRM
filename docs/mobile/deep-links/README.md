# Deep linking (M4 + D1-13i)

**Path:** `lib/core/navigation/deep_links/`

## Schemes

| Type | Example |
|------|---------|
| Custom | `ashraak://profile` |
| Universal | `https://app.ashraak.example/files/{id}/preview` |

## Supported targets

| Link | Route |
|------|-------|
| Password reset | `/reset-password` + query |
| Invitation accept | `/unauthorized` + query |
| Audit entry | `/audit` |
| Notification open | `/notifications/preferences` |
| Files preview | `/files/{id}/preview` |
| Profile | `/profile` |

### CCTV push deep links (D1-13i)

Notification payloads include `DeepLink` (`ashraak://…`). Parser routes to existing GoRouter paths:

| Notification | Deep link | Mobile route |
|--------------|-----------|--------------|
| Ticket | `ashraak://cctv/customer/tickets/{id}` | `/cctv/customer/tickets/{id}` |
| Invoice | `ashraak://cctv/customer/invoices/{id}` | `/cctv/customer/invoices/{id}` |
| Visit scheduled | `ashraak://cctv/customer/visits` | `/cctv/customer/visits` |
| Visit completed / approved | `ashraak://cctv/customer/service-history` | `/cctv/customer/service-history` |
| Engineer visit assigned | `ashraak://cctv/engineer/visits/{id}/report` | `/cctv/engineer/visits/{id}/report` |
| AMC renewal / expiry | `ashraak://cctv/customer/amc` | `/cctv/customer/amc` |

Parsed by `DeepLinkParser`, routed via `DeepLinkHandler` + existing GoRouter — **no duplicate routes**.

Push notification taps use the same handler (`AppShellPage.onNotificationTap`).

See [d1-13i-final-scope-completion-report.md](../../project/d1-13i-final-scope-completion-report.md).

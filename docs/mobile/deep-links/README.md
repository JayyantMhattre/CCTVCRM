# Deep linking (M4)

**Path:** `lib/core/navigation/deep_links/`

## Schemes

| Type | Example |
|------|---------|
| Custom | `ashraak://profile` |
| Universal | `https://app.ashraak.example/files/{id}/preview` |

## Supported targets

| Link | Route |
|------|-------|
| Password reset | `/unauthorized` + query |
| Invitation accept | `/unauthorized` + query |
| Audit entry | `/audit` |
| Notification open | `/notifications/preferences` |
| Files preview | `/files/{id}/preview` |
| Profile | `/profile` |

Parsed by `DeepLinkParser`, routed via `DeepLinkHandler` + existing GoRouter — **no duplicate routes**.

Push notification taps use the same handler.

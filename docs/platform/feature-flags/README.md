# Feature flags (foundation)

Minimal config-backed feature gating for future tenant/plan controls — **not** a full feature-flag framework.

**Contract:** `IFeatureFlagService` in `SharedKernel.Contracts`

**Implementation:** `ConfigFeatureFlagService` in host

## Usage

```csharp
if (await featureFlags.IsEnabledAsync("notifications.email", tenantId, ct))
    await notificationService.SendEmailAsync(...);
```

## Configuration

```json
"Features": {
  "Flags": {
    "notifications.email": true,
    "audit.security-events": true
  },
  "TenantOverrides": {
    "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee": {
      "notifications.email": false
    }
  }
}
```

## Related docs

- [architecture.md](./architecture.md)
- [future-extension.md](./future-extension.md)

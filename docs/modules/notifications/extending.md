# Notifications — Extending

## Add a template

1. Add `Templates/{key}.txt` under `Ashraak.Api`
2. Add constant to `EmailTemplates.cs`
3. Create `INotificationHandler<TEvent>` in `Notifications.Application`
4. Document in [events.md](./events.md)

## Add provider

1. Implement `IEmailProvider` in Infrastructure
2. Register in `NotificationsModule`
3. Extend `NotificationService.ResolveProvider`
4. Document in [provider-setup.md](./provider-setup.md)

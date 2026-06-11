# Notifications — Registration

## DI

`NotificationsModule.AddNotificationsModule(configuration)` in `ModuleExtensions.cs` after Audit.

Registers:

- `INotificationService` → `NotificationService`
- `ConsoleEmailProvider`, `SmtpEmailProvider`
- MediatR handlers from `Ashraak.Notifications.Application`

## Endpoints

`MapNotificationsEndpoints()` → `GET /api/v1/notifications/health` (AdminOnly)

## Disable

Comment out `AddNotificationsModule` and `MapNotificationsEndpoints` in `ModuleExtensions.cs`; remove project references from `Ashraak.Api.csproj`.

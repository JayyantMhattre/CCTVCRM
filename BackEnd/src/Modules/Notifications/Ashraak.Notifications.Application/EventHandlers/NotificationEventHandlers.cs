using Ashraak.SharedKernel.Contracts.Notifications;
using Ashraak.SharedKernel.Contracts.Notifications.Interfaces;
using Ashraak.SharedKernel.Contracts.Tenant.Events;
using Ashraak.SharedKernel.Contracts.Users.Events;
using MediatR;

namespace Ashraak.Notifications.Application.EventHandlers;

internal sealed class UserCreatedNotificationHandler(INotificationService notifications)
    : INotificationHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken) =>
        notifications.SendEmailAsync(
            EmailTemplates.Welcome,
            notification.Email,
            new Dictionary<string, string>
            {
                ["DisplayName"] = notification.DisplayName,
                ["TenantId"] = notification.TenantId.ToString("D"),
                ["UserId"] = notification.UserId.ToString("D")
            },
            cancellationToken);
}

internal sealed class UserInvitedNotificationHandler(INotificationService notifications)
    : INotificationHandler<UserInvitedEvent>
{
    public Task Handle(UserInvitedEvent notification, CancellationToken cancellationToken) =>
        notifications.SendEmailAsync(
            EmailTemplates.Invitation,
            notification.Email,
            new Dictionary<string, string>
            {
                ["InvitationToken"] = notification.InvitationToken,
                ["TenantId"] = notification.TenantId.ToString("D"),
                ["ExpiresOnUtc"] = notification.ExpiresOnUtc.ToString("O")
            },
            cancellationToken);
}

internal sealed class TenantProvisionedNotificationHandler(INotificationService notifications)
    : INotificationHandler<TenantProvisionedEvent>
{
    public Task Handle(TenantProvisionedEvent notification, CancellationToken cancellationToken) =>
        notifications.SendEmailAsync(
            EmailTemplates.Welcome,
            "owner@tenant.local",
            new Dictionary<string, string>
            {
                ["TenantName"] = notification.Name,
                ["Slug"] = notification.Slug,
                ["Plan"] = notification.Plan.ToString(),
                ["OwnerUserId"] = notification.OwnerUserId.ToString("D")
            },
            cancellationToken);
}

internal sealed class TenantSuspendedNotificationHandler(INotificationService notifications)
    : INotificationHandler<TenantSuspendedEvent>
{
    public Task Handle(TenantSuspendedEvent notification, CancellationToken cancellationToken) =>
        notifications.SendEmailAsync(
            EmailTemplates.TenantSuspended,
            "admin@tenant.local",
            new Dictionary<string, string>
            {
                ["TenantId"] = notification.TenantId.ToString("D"),
                ["Reason"] = notification.Reason
            },
            cancellationToken);
}

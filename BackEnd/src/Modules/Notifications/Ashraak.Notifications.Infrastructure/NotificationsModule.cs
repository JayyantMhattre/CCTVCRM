using Ashraak.Notifications.Application;
using Ashraak.Notifications.Infrastructure.Providers;
using Ashraak.Notifications.Infrastructure.Services;
using Ashraak.Notifications.Infrastructure.Templates;
using Ashraak.SharedKernel.Contracts.Notifications.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Notifications.Infrastructure;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<NotificationOptions>(configuration.GetSection("Notifications"));

        services.AddSingleton<FileEmailTemplateRenderer>();
        services.AddSingleton<ConsoleEmailProvider>();
        services.AddSingleton<SmtpEmailProvider>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(NotificationsApplicationAnchor).Assembly));

        return services;
    }
}

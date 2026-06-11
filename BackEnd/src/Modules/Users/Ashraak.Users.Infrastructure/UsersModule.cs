using Ashraak.SharedKernel.Contracts.Users.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Users.Application.Commands.CreateUserProfile;
using Ashraak.Users.Domain.Repositories;
using Ashraak.Users.Infrastructure.Persistence;
using Ashraak.Users.Infrastructure.Persistence.Repositories;
using Ashraak.Users.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Users.Infrastructure;

/// <summary>
/// DI composition root for the Users module.
/// Registers <c>UsersDbContext</c>, <c>IUserProfileRepository</c>, the cross-module
/// <c>IUserService</c>, and all MediatR handlers for the Users Application assembly.
/// </summary>
public static class UsersModule
{
    /// <summary>
    /// Registers all Users module services with the DI container.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configuration">
    /// Application configuration used to resolve <c>ConnectionStrings:Users</c>
    /// or <c>ConnectionStrings:DefaultConnection</c>.
    /// </param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no connection string is configured.</exception>
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Users")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Users database connection string is required.");

        services.AddDbContext<UsersDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "users");
                npgsql.EnableRetryOnFailure(3);
            });

            // Resolve any registered EF interceptors at runtime (e.g. audit entity change interceptor).
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateUserProfileCommand).Assembly));

        return services;
    }
}

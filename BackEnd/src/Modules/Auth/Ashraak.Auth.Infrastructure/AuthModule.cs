using Ashraak.Auth.Application.Abstractions;
using Ashraak.Auth.Application.Commands.RegisterUser;
using Ashraak.Auth.Domain.Repositories;
using Ashraak.Auth.Infrastructure.Persistence;
using Ashraak.Auth.Infrastructure.Persistence.Repositories;
using Ashraak.Auth.Infrastructure.Security;
using Ashraak.Auth.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Domain.Events;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Auth.Infrastructure;

/// <summary>
/// DI composition root for the Auth module.
/// Registers the module's DbContext, ASP.NET Core Identity, OpenIddict, repositories,
/// password hasher, MediatR handlers, and FluentValidation validators.
/// </summary>
/// <remarks>
/// Called once from <c>Ashraak.Api.Program</c> during application startup.
/// The method follows the "module registration" pattern: each module exposes a single
/// static extension method that owns all its DI registrations, preventing leakage into
/// the host project.
/// </remarks>
public static class AuthModule
{
    /// <summary>
    /// Registers all Auth module services with the DI container.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configuration">
    /// Application configuration used to read the <c>ConnectionStrings:Auth</c> or
    /// <c>ConnectionStrings:DefaultConnection</c> value.
    /// </param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no database connection string is found.</exception>
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Auth")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Auth database connection string is required.");

        services.AddDbContext<AuthDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "auth");
                npgsql.EnableRetryOnFailure(3);
            });

            // Attach all registered EF Core interceptors (including Audit interceptor) without
            // introducing a hard compile-time dependency on the Audit module.
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = false;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AuthDbContext>();

        services.AddOpenIddict()
            .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<AuthDbContext>())
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/connect/authorize")
                       .SetTokenEndpointUris("/connect/token")
                       .SetIntrospectionEndpointUris("/connect/introspect")
                       .SetLogoutEndpointUris("/connect/logout");

                options.AllowAuthorizationCodeFlow()
                       .AllowPasswordFlow()
                       .AllowClientCredentialsFlow()
                       .AllowRefreshTokenFlow();

                options.AcceptAnonymousClients();

                options.AddEphemeralEncryptionKey()
                       .AddEphemeralSigningKey();

                options.UseAspNetCore()
                       .EnableTokenEndpointPassthrough()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableLogoutEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        services.AddAuthentication()
            .AddCookie("Auth.External", options =>
            {
                options.Cookie.Name = "ashraak.auth.external";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
                options.SignInScheme = "Auth.External";
                options.CallbackPath = "/api/auth/sso/google/callback";
                options.SaveTokens = true;
            })
            .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = configuration["Authentication:Microsoft:ClientId"] ?? string.Empty;
                options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"] ?? string.Empty;
                options.SignInScheme = "Auth.External";
                options.CallbackPath = "/api/auth/sso/microsoft/callback";
                options.SaveTokens = true;
            });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AuthDbContext>());
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<AuthAuthorizationRepository>();
        services.AddScoped<IRoleAssignmentService, RoleAssignmentService>();
        services.AddScoped<IInvitationTokenService, InvitationTokenService>();
        services.AddScoped<ITotpService, TotpService>();
        services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        services.AddScoped<IAuthPermissionChecker, AuthPermissionChecker>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

        services.AddValidatorsFromAssembly(typeof(RegisterUserCommand).Assembly);

        return services;
    }
}

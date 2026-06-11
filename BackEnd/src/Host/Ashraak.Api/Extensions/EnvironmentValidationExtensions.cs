using Ashraak.Api.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ashraak.Api.Extensions;

/// <summary>
/// Fail-fast startup validation for required configuration and dependencies.
/// </summary>
public static class EnvironmentValidationExtensions
{
    public const string SectionName = "EnvironmentValidation";

    /// <summary>
    /// Validates required configuration before the host accepts traffic.
    /// Throws <see cref="InvalidOperationException"/> with a clear message when validation fails.
    /// </summary>
    public static void ValidateAshraakEnvironment(
        this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        var errors = new List<string>();

        RequireConnectionString(configuration, "DefaultConnection", errors);
        RequireConnectionString(configuration, "Auth", errors);
        RequireConnectionString(configuration, "Tenant", errors);
        RequireConnectionString(configuration, "Users", errors);
        RequireConnectionString(configuration, "Redis", errors);
        RequireConnectionString(configuration, "MongoDB", errors);

        var outboxSection = configuration.GetSection("Outbox");
        if (!TimeSpan.TryParse(outboxSection["PollInterval"], out var poll) || poll <= TimeSpan.Zero)
            errors.Add("Outbox:PollInterval must be a positive duration (e.g. 00:00:05).");

        if (!int.TryParse(outboxSection["BatchSize"], out var batch) || batch <= 0)
            errors.Add("Outbox:BatchSize must be a positive integer.");

        var notifications = configuration.GetSection("Notifications");
        if (string.IsNullOrWhiteSpace(notifications["Provider"]))
            errors.Add("Notifications:Provider is required (e.g. console, smtp).");

        if (string.IsNullOrWhiteSpace(notifications["TemplatesPath"]))
            errors.Add("Notifications:TemplatesPath is required.");

        if (environment.IsProduction())
        {
            var seqUrl = configuration["Seq:Url"];
            if (string.IsNullOrWhiteSpace(seqUrl))
                errors.Add("Seq:Url is required in Production.");

            var signingKey = configuration["Authentication:Jwt:SigningKey"]
                             ?? configuration["OpenIddict:SigningKey"];
            if (string.IsNullOrWhiteSpace(signingKey))
                errors.Add("Production requires a stable signing key (Authentication:Jwt:SigningKey or OpenIddict:SigningKey). Ephemeral dev keys are not allowed.");
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Environment validation failed. Fix configuration before starting the host:" +
                Environment.NewLine + string.Join(Environment.NewLine, errors.Select(e => " - " + e)));
        }
    }

    private static void RequireConnectionString(
        IConfiguration configuration,
        string name,
        List<string> errors)
    {
        var value = configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(value))
            errors.Add($"ConnectionStrings:{name} is required.");
    }
}

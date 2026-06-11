namespace Ashraak.ApiKeys.Application;

public sealed class ApiKeysOptions
{
    public const string SectionName = "ApiKeys";

    /// <summary>Environment segment in key format (dev, qa, uat, prod).</summary>
    public string Environment { get; set; } = "prod";

    /// <summary>Default key lifetime in days (0 = no expiry).</summary>
    public int DefaultExpiryDays { get; set; }

    /// <summary>Accept X-API-Key header for authentication.</summary>
    public bool AllowHeaderAuthentication { get; set; } = true;

    /// <summary>Accept Authorization: Bearer {api_key} for authentication.</summary>
    public bool AllowBearerAuthentication { get; set; } = true;

    /// <summary>Per-key rate limit (requests per window). 0 = use global defaults only.</summary>
    public int PerKeyRateLimit { get; set; } = 1000;

    public int PerKeyRateLimitWindowSeconds { get; set; } = 60;
}

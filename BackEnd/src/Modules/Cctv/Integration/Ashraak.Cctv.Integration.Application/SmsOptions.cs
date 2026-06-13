namespace Ashraak.Cctv.Integration.Application;

/// <summary>Configuration-driven SMS provider settings (no vendor lock-in).</summary>
public sealed class SmsOptions
{
    public const string SectionName = "Sms";

    /// <summary>Provider mode: <c>console</c> (log) or <c>http</c> (generic REST gateway).</summary>
    public string Provider { get; set; } = "console";

    public SmsHttpOptions Http { get; set; } = new();
}

public sealed class SmsHttpOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKeyHeader { get; set; } = "Authorization";
    public string ApiKeyValue { get; set; } = string.Empty;
    public string PhoneNumberField { get; set; } = "to";
    public string MessageField { get; set; } = "message";
}

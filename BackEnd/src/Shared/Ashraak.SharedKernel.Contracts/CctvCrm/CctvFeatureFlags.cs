namespace Ashraak.SharedKernel.Contracts.CctvCrm;

/// <summary>
/// Feature flag keys for Aarvii CCTV business modules.
/// Evaluated via platform <see cref="FeatureFlags.Interfaces.IFeatureFlagService"/> (config section <c>Features:Flags</c>).
/// </summary>
public static class CctvFeatureFlags
{
    /// <summary>Master gate for all CCTV APIs and UI (health always available when registered).</summary>
    public const string Enabled = "cctv.enabled";

    public const string LeadsEnabled = "cctv.leads.enabled";
    public const string CustomersEnabled = "cctv.customers.enabled";
    public const string AmcEnabled = "cctv.amc.enabled";
    public const string ServiceEnabled = "cctv.service.enabled";
    public const string TicketsEnabled = "cctv.tickets.enabled";
    public const string EngineersEnabled = "cctv.engineers.enabled";
    public const string InvoicesEnabled = "cctv.invoices.enabled";
    public const string ReportingEnabled = "cctv.reporting.enabled";
    public const string CustomerPortalEnabled = "cctv.portal.customer.enabled";
    public const string EngineerPortalEnabled = "cctv.portal.engineer.enabled";
    public const string CustomerMobileEnabled = "cctv.mobile.customer.enabled";
    public const string EngineerMobileEnabled = "cctv.mobile.engineer.enabled";
    public const string SmsEnabled = "cctv.integrations.sms.enabled";
    public const string PdfEnabled = "cctv.integrations.pdf.enabled";

    /// <summary>All module flag keys for seeding config and documentation.</summary>
    public static readonly IReadOnlyList<string> All =
    [
        Enabled,
        LeadsEnabled,
        CustomersEnabled,
        AmcEnabled,
        ServiceEnabled,
        TicketsEnabled,
        EngineersEnabled,
        InvoicesEnabled,
        ReportingEnabled,
        CustomerPortalEnabled,
        EngineerPortalEnabled,
        CustomerMobileEnabled,
        EngineerMobileEnabled,
        SmsEnabled,
        PdfEnabled
    ];
}

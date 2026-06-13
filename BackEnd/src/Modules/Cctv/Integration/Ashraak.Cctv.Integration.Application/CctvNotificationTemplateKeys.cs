namespace Ashraak.Cctv.Integration.Application;

/// <summary>CCTV email template keys under <c>Templates/cctv/</c>.</summary>
public static class CctvNotificationTemplateKeys
{
    public const string LeadCreated = "cctv/cctv-lead-created";
    public const string LeadConverted = "cctv/cctv-lead-converted";
    public const string CustomerWelcome = "cctv/cctv-customer-welcome";
    public const string AmcRenewalRequested = "cctv/cctv-amc-renewal-requested";
    public const string AmcExpiryReminder = "cctv/cctv-amc-expiry-reminder";
    public const string VisitScheduled = "cctv/cctv-visit-scheduled";
    public const string VisitCompleted = "cctv/cctv-visit-completed";
    public const string VisitApproved = "cctv/cctv-visit-approved";
    public const string TicketCreated = "cctv/cctv-ticket-created";
    public const string TicketAssigned = "cctv/cctv-ticket-assigned";
    public const string TicketClosed = "cctv/cctv-ticket-closed";
    public const string InvoiceGenerated = "cctv/cctv-invoice-generated";
}

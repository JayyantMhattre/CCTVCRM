namespace Ashraak.SharedKernel.Contracts.CctvCrm.Enums;

/// <summary>PascalCase status strings matching <c>cctv_lead.leads.status</c> CHECK constraint.</summary>
public static class LeadStatusContract
{
    public const string New = nameof(New);
    public const string Contacted = nameof(Contacted);
    public const string Qualified = nameof(Qualified);
    public const string QuotationSent = nameof(QuotationSent);
    public const string Negotiation = nameof(Negotiation);
    public const string Won = nameof(Won);
    public const string Lost = nameof(Lost);
    public const string Converted = nameof(Converted);
}

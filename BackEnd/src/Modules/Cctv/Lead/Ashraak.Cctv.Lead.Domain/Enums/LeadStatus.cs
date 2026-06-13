namespace Ashraak.Cctv.Lead.Domain.Enums;

/// <summary>Pipeline status values (BR-LEAD-01).</summary>
public enum LeadStatus
{
    New = 0,
    Contacted = 1,
    Qualified = 2,
    QuotationSent = 3,
    Negotiation = 4,
    Won = 5,
    Lost = 6,
    Converted = 7
}

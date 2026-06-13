using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

/// <summary>Contact person for a site (max 3 per site, BR-STRUCT-03).</summary>
public sealed class SiteContact : Entity<SiteContactId>
{
    private SiteContact(SiteContactId id) : base(id) { }

    public SiteId SiteId { get; private set; }
    public short ContactSlot { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Designation { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    internal static SiteContact Create(
        SiteContactId id,
        SiteId siteId,
        short contactSlot,
        string name,
        string? designation,
        string phone,
        string? email,
        bool isPrimary,
        Guid createdBy)
    {
        return new SiteContact(id)
        {
            SiteId = siteId,
            ContactSlot = contactSlot,
            Name = name.Trim(),
            Designation = string.IsNullOrWhiteSpace(designation) ? null : designation.Trim(),
            Phone = phone.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            IsPrimary = isPrimary,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

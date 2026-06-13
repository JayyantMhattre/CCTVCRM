using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

/// <summary>Document linked to a site via platform FileId.</summary>
public sealed class SiteDocument : Entity<SiteDocumentId>
{
    private SiteDocument(SiteDocumentId id) : base(id) { }

    public SiteId SiteId { get; private set; }
    public Guid FileId { get; private set; }
    public SiteDocumentType DocumentType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    internal static SiteDocument Create(
        SiteDocumentId id,
        SiteId siteId,
        Guid fileId,
        SiteDocumentType documentType,
        string title,
        Guid createdBy)
    {
        return new SiteDocument(id)
        {
            SiteId = siteId,
            FileId = fileId,
            DocumentType = documentType,
            Title = title.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    internal void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        _ = deletedBy;
    }
}

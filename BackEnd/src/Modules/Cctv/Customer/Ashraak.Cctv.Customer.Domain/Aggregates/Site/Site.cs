using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site.Events;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Customer.Domain.Aggregates.Site;

/// <summary>Site aggregate root (schema <c>cctv_customer.sites</c>).</summary>
public sealed class Site : AggregateRoot<SiteId>
{
    private const int MaxContacts = 3;

    private readonly List<SiteContact> _contacts = [];
    private readonly List<SiteDocument> _documents = [];

    private Site(SiteId id) : base(id) { }

    public CustomerId CustomerId { get; private set; }
    public string SiteNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public SiteStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public uint RowVersion { get; private set; }
    public SiteAssetSummary? AssetSummary { get; private set; }

    public IReadOnlyList<SiteContact> Contacts => _contacts.AsReadOnly();

    public IReadOnlyList<SiteDocument> Documents => _documents.AsReadOnly();

    public static Site CreateManual(
        SiteId id,
        CustomerId customerId,
        string siteNumber,
        string name,
        string address,
        string city,
        Guid createdBy)
    {
        var site = new Site(id)
        {
            CustomerId = customerId,
            SiteNumber = siteNumber,
            Name = name.Trim(),
            Address = address.Trim(),
            City = city.Trim(),
            Status = SiteStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        site.RaiseDomainEvent(new SiteCreatedDomainEvent(
            id.Value, siteNumber, customerId.Value, site.Name));

        return site;
    }

    public static Site CreateFromLead(
        SiteId id,
        CustomerId customerId,
        string siteNumber,
        string name,
        string address,
        string city,
        Guid createdBy)
    {
        return CreateManual(id, customerId, siteNumber, name, address, city, createdBy);
    }

    public void UpdateDetails(string name, string address, string city, Guid updatedBy)
    {
        EnsureActive();

        Name = name.Trim();
        Address = address.Trim();
        City = city.Trim();
        Touch(updatedBy);

        RaiseDomainEvent(new SiteUpdatedDomainEvent(Id.Value, SiteNumber, updatedBy));
    }

    public void ChangeStatus(SiteStatus toStatus, Guid changedBy)
    {
        if (Status == toStatus)
            return;

        if (toStatus == SiteStatus.Inactive && Status == SiteStatus.Active)
        {
            Status = SiteStatus.Inactive;
            Touch(changedBy);
            RaiseDomainEvent(new SiteDeactivatedDomainEvent(Id.Value, SiteNumber, changedBy));
            return;
        }

        if (toStatus == SiteStatus.Active && Status == SiteStatus.Inactive)
        {
            Status = SiteStatus.Active;
            Touch(changedBy);
            RaiseDomainEvent(new SiteUpdatedDomainEvent(Id.Value, SiteNumber, changedBy));
            return;
        }

        throw new InvalidOperationException($"Transition from {Status} to {toStatus} is not allowed.");
    }

    public void ReplaceContacts(IReadOnlyList<SiteContactInput> contacts, Guid updatedBy)
    {
        EnsureActive();

        if (contacts.Count > MaxContacts)
            throw new InvalidOperationException("A site can have at most 3 contacts.");

        if (contacts.Count > 0)
        {
            var primaryCount = contacts.Count(c => c.IsPrimary);
            if (primaryCount != 1)
                throw new InvalidOperationException("Exactly one contact must be primary when contacts are specified.");
        }

        _contacts.Clear();

        for (var i = 0; i < contacts.Count; i++)
        {
            var input = contacts[i];
            _contacts.Add(SiteContact.Create(
                SiteContactId.New(),
                Id,
                (short)(i + 1),
                input.Name,
                input.Designation,
                input.Phone,
                input.Email,
                input.IsPrimary,
                updatedBy));
        }

        Touch(updatedBy);
        RaiseDomainEvent(new SiteContactChangedDomainEvent(
            Id.Value, SiteNumber, contacts.Count, updatedBy));
    }

    public SiteDocument LinkDocument(
        Guid fileId,
        SiteDocumentType documentType,
        string title,
        Guid createdBy)
    {
        EnsureActive();

        var document = SiteDocument.Create(
            SiteDocumentId.New(),
            Id,
            fileId,
            documentType,
            title,
            createdBy);

        _documents.Add(document);
        Touch(createdBy);
        RaiseDomainEvent(new SiteUpdatedDomainEvent(Id.Value, SiteNumber, createdBy));

        return document;
    }

    public void RemoveDocument(SiteDocumentId documentId, Guid removedBy)
    {
        EnsureActive();

        var document = _documents.FirstOrDefault(d => d.Id == documentId && !d.IsDeleted);
        if (document is null)
            throw new InvalidOperationException("Document not found on this site.");

        document.SoftDelete(removedBy);
        Touch(removedBy);
        RaiseDomainEvent(new SiteUpdatedDomainEvent(Id.Value, SiteNumber, removedBy));
    }

    public void UpsertAssetSummary(
        int cameraCount,
        int dvrCount,
        int nvrCount,
        int hardDiskCount,
        int switchCount,
        int routerCount,
        int monitorCount,
        string? brand,
        string? model,
        string? remarks,
        Guid userId)
    {
        EnsureActive();

        if (AssetSummary is null)
        {
            AssetSummary = SiteAssetSummary.Create(
                SiteAssetSummaryId.New(),
                Id,
                cameraCount,
                dvrCount,
                nvrCount,
                hardDiskCount,
                switchCount,
                routerCount,
                monitorCount,
                brand,
                model,
                remarks,
                userId);
        }
        else
        {
            AssetSummary.Update(
                cameraCount,
                dvrCount,
                nvrCount,
                hardDiskCount,
                switchCount,
                routerCount,
                monitorCount,
                brand,
                model,
                remarks,
                userId);
        }

        Touch(userId);
        RaiseDomainEvent(new SiteAssetSummaryUpdatedDomainEvent(Id.Value, SiteNumber, userId));
    }

    public void UpdateAssetSummary(
        int cameraCount,
        int dvrCount,
        int nvrCount,
        int hardDiskCount,
        int switchCount,
        int routerCount,
        int monitorCount,
        string? brand,
        string? model,
        string? remarks,
        Guid userId)
    {
        EnsureActive();

        if (AssetSummary is null)
            throw new InvalidOperationException("Asset summary does not exist for this site.");

        AssetSummary.Update(
            cameraCount,
            dvrCount,
            nvrCount,
            hardDiskCount,
            switchCount,
            routerCount,
            monitorCount,
            brand,
            model,
            remarks,
            userId);

        Touch(userId);
        RaiseDomainEvent(new SiteAssetSummaryUpdatedDomainEvent(Id.Value, SiteNumber, userId));
    }

    public void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        Touch(deletedBy);
    }

    private void EnsureActive()
    {
        if (Status == SiteStatus.Inactive)
            throw new InvalidOperationException("Inactive sites are read-only.");
    }

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }
}

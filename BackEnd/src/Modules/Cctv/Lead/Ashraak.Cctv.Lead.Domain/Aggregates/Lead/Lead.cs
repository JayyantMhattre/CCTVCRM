using Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Events;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadActivity;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadAttachment;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadRemark;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Lead.Domain.Aggregates.Lead;

/// <summary>Sales prospect aggregate root (schema <c>cctv_lead.leads</c>).</summary>
public sealed class Lead : AggregateRoot<LeadId>
{
    private static readonly IReadOnlyDictionary<LeadStatus, LeadStatus[]> AllowedTransitions =
        new Dictionary<LeadStatus, LeadStatus[]>
        {
            [LeadStatus.New] = [LeadStatus.Contacted, LeadStatus.Lost],
            [LeadStatus.Contacted] = [LeadStatus.Qualified, LeadStatus.Lost],
            [LeadStatus.Qualified] = [LeadStatus.QuotationSent, LeadStatus.Lost],
            [LeadStatus.QuotationSent] = [LeadStatus.Negotiation, LeadStatus.Lost],
            [LeadStatus.Negotiation] = [LeadStatus.Won, LeadStatus.Lost],
            [LeadStatus.Won] = [],
            [LeadStatus.Lost] = [],
            [LeadStatus.Converted] = []
        };

    private readonly List<LeadActivity.LeadActivity> _activities = [];
    private readonly List<LeadRemark.LeadRemark> _remarks = [];
    private readonly List<LeadAttachment.LeadAttachment> _attachments = [];

    private Lead(LeadId id) : base(id) { }

    public string LeadNumber { get; private set; } = string.Empty;
    public LeadSource Source { get; private set; }
    public LeadStatus Status { get; private set; }
    public string ContactName { get; private set; } = string.Empty;
    public string? OrganizationName { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? RequirementSummary { get; private set; }
    public Guid? OwnerUserId { get; private set; }
    public Guid? ConvertedCustomerId { get; private set; }
    public Guid? ConvertedSiteId { get; private set; }
    public Guid? ConvertedContractId { get; private set; }
    public DateTime? ConvertedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public uint RowVersion { get; private set; }

    public IReadOnlyList<LeadActivity.LeadActivity> Activities => _activities.AsReadOnly();
    public IReadOnlyList<LeadRemark.LeadRemark> Remarks => _remarks.AsReadOnly();
    public IReadOnlyList<LeadAttachment.LeadAttachment> Attachments => _attachments.AsReadOnly();

    public static Lead CreateFromInquiry(
        LeadId id,
        string leadNumber,
        LeadSource source,
        string contactName,
        string? organizationName,
        string email,
        string phone,
        string city,
        string? address,
        string? requirementSummary,
        Guid createdBy)
    {
        var lead = new Lead(id)
        {
            LeadNumber = leadNumber,
            Source = source,
            Status = LeadStatus.New,
            ContactName = contactName.Trim(),
            OrganizationName = NormalizeOptional(organizationName),
            Email = email.Trim(),
            Phone = phone.Trim(),
            City = city.Trim(),
            Address = NormalizeOptional(address),
            RequirementSummary = NormalizeOptional(requirementSummary),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        lead.RaiseDomainEvent(new LeadCreatedDomainEvent(
            id.Value, leadNumber, source, lead.ContactName, lead.Email));

        return lead;
    }

    public static Lead CreateManual(
        LeadId id,
        string leadNumber,
        LeadSource source,
        string contactName,
        string? organizationName,
        string email,
        string phone,
        string city,
        string? address,
        string? requirementSummary,
        Guid? ownerUserId,
        Guid createdBy)
    {
        var lead = CreateFromInquiry(
            id,
            leadNumber,
            source,
            contactName,
            organizationName,
            email,
            phone,
            city,
            address,
            requirementSummary,
            createdBy);

        lead.OwnerUserId = ownerUserId;
        return lead;
    }

    public void UpdateDetails(
        string contactName,
        string? organizationName,
        string email,
        string phone,
        string city,
        string? address,
        string? requirementSummary,
        Guid? ownerUserId,
        Guid updatedBy)
    {
        EnsureNotTerminal();

        ContactName = contactName.Trim();
        OrganizationName = NormalizeOptional(organizationName);
        Email = email.Trim();
        Phone = phone.Trim();
        City = city.Trim();
        Address = NormalizeOptional(address);
        RequirementSummary = NormalizeOptional(requirementSummary);
        OwnerUserId = ownerUserId;
        Touch(updatedBy);
    }

    public LeadActivity.LeadActivity ChangeStatus(
        LeadStatus toStatus,
        Guid performedBy,
        string? notes)
    {
        if (toStatus == LeadStatus.Converted)
            throw new InvalidOperationException("Use Convert to reach Converted status.");

        if (Status == LeadStatus.Lost || Status == LeadStatus.Converted)
            throw new InvalidOperationException($"Lead in status {Status} cannot change status.");

        if (!AllowedTransitions[Status].Contains(toStatus))
            throw new InvalidOperationException($"Transition from {Status} to {toStatus} is not allowed.");

        var fromStatus = Status;
        Status = toStatus;
        Touch(performedBy);

        var activity = LeadActivity.LeadActivity.CreateStatusChange(
            LeadActivityId.New(),
            Id,
            fromStatus,
            toStatus,
            notes ?? $"Status changed to {toStatus}",
            performedBy);

        _activities.Add(activity);

        if (toStatus == LeadStatus.Lost)
        {
            RaiseDomainEvent(new LeadLostDomainEvent(Id.Value, LeadNumber, performedBy));
        }
        else
        {
            RaiseDomainEvent(new LeadStatusChangedDomainEvent(
                Id.Value, LeadNumber, fromStatus, toStatus, performedBy));
        }

        return activity;
    }

    public LeadActivity.LeadActivity AddActivity(
        LeadActivityType activityType,
        string description,
        LeadStatus? fromStatus,
        LeadStatus? toStatus,
        Guid performedBy)
    {
        EnsureNotTerminal();

        var activity = LeadActivity.LeadActivity.Create(
            LeadActivityId.New(),
            Id,
            activityType,
            description,
            fromStatus,
            toStatus,
            performedBy);

        _activities.Add(activity);
        Touch(performedBy);
        return activity;
    }

    public LeadRemark.LeadRemark AddRemark(string text, Guid createdBy)
    {
        EnsureNotTerminal();

        var remark = LeadRemark.LeadRemark.Create(LeadRemarkId.New(), Id, text, createdBy);
        _remarks.Add(remark);
        Touch(createdBy);
        return remark;
    }

    public LeadAttachment.LeadAttachment LinkAttachment(Guid fileId, string title, Guid createdBy)
    {
        EnsureNotTerminal();

        var attachment = LeadAttachment.LeadAttachment.Create(
            LeadAttachmentId.New(), Id, fileId, title, createdBy);

        _attachments.Add(attachment);
        Touch(createdBy);
        return attachment;
    }

    public void RemoveAttachment(LeadAttachmentId attachmentId, Guid removedBy)
    {
        EnsureNotTerminal();

        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId && !a.IsDeleted)
            ?? throw new InvalidOperationException("Attachment not found.");

        attachment.SoftDelete(removedBy);
        Touch(removedBy);
    }

    public void Convert(Guid customerId, Guid siteId, Guid contractId, Guid convertedBy)
    {
        if (Status != LeadStatus.Won)
            throw new InvalidOperationException("Only Won leads can be converted.");

        Status = LeadStatus.Converted;
        ConvertedCustomerId = customerId;
        ConvertedSiteId = siteId;
        ConvertedContractId = contractId;
        ConvertedAtUtc = DateTime.UtcNow;
        Touch(convertedBy);

        var activity = LeadActivity.LeadActivity.CreateStatusChange(
            LeadActivityId.New(),
            Id,
            LeadStatus.Won,
            LeadStatus.Converted,
            "Lead converted to customer",
            convertedBy);

        _activities.Add(activity);

        RaiseDomainEvent(new LeadConvertedDomainEvent(
            Id.Value,
            LeadNumber,
            customerId,
            siteId,
            contractId,
            convertedBy));
    }

    public void SoftDelete(Guid deletedBy)
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        Touch(deletedBy);
    }

    private void EnsureNotTerminal()
    {
        if (Status is LeadStatus.Lost or LeadStatus.Converted)
            throw new InvalidOperationException($"Lead in status {Status} is read-only.");
    }

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

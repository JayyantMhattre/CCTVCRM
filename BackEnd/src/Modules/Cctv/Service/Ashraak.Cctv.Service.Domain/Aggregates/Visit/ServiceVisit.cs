using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit.Events;
using Ashraak.Cctv.Service.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit;

/// <summary>Service visit aggregate root (schema <c>cctv_service.service_visits</c>).</summary>
public sealed class ServiceVisit : AggregateRoot<ServiceVisitId>
{
    private const int MinRemarksLength = 20;

    private readonly List<VisitPhoto> _photos = [];
    private readonly List<VisitApproval> _approvals = [];
    private readonly List<VisitAttachment> _attachments = [];

    private ServiceVisit(ServiceVisitId id) : base(id) { }

    public ServiceScheduleId ServiceScheduleId { get; private set; }
    public Guid EngineerId { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? VisitRemarks { get; private set; }
    public VisitReportStatus ReportStatus { get; private set; }
    public bool IsCustomerVisible { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public uint RowVersion { get; private set; }

    public VisitLocation? Location { get; private set; }
    public VisitSignature? Signature { get; private set; }

    public IReadOnlyList<VisitPhoto> Photos => _photos.AsReadOnly();
    public IReadOnlyList<VisitApproval> Approvals => _approvals.AsReadOnly();
    public IReadOnlyList<VisitAttachment> Attachments => _attachments.AsReadOnly();

    public bool HasSelfie => _photos.Any(p => p.Category == VisitPhotoCategory.Selfie);

    public bool HasBeforeDuringAfterPhoto =>
        _photos.Any(p => p.Category is VisitPhotoCategory.Before
            or VisitPhotoCategory.During
            or VisitPhotoCategory.After);

    public bool HasGps => Location is not null;

    public bool HasSignature => Signature is not null;

    public bool HasMinimumRemarks =>
        !string.IsNullOrWhiteSpace(VisitRemarks) && VisitRemarks.Trim().Length >= MinRemarksLength;

    public static ServiceVisit CreateDraft(
        ServiceVisitId id,
        ServiceScheduleId scheduleId,
        Guid engineerId,
        Guid createdBy)
    {
        return new ServiceVisit(id)
        {
            ServiceScheduleId = scheduleId,
            EngineerId = engineerId,
            ReportStatus = VisitReportStatus.Draft,
            IsCustomerVisible = false,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };
    }

    public void Start(DateTime startedAtUtc, Guid startedBy)
    {
        if (StartedAtUtc.HasValue)
            throw new InvalidOperationException("Visit has already been started.");

        if (ReportStatus is not VisitReportStatus.Draft and not VisitReportStatus.Returned)
            throw new InvalidOperationException($"Visit in status {ReportStatus} cannot be started.");

        StartedAtUtc = startedAtUtc;
        if (ReportStatus == VisitReportStatus.Returned)
            ReportStatus = VisitReportStatus.Draft;

        Touch(startedBy);

        RaiseDomainEvent(new VisitStartedDomainEvent(
            Id.Value, ServiceScheduleId.Value, EngineerId, startedAtUtc));
    }

    public void UpdateRemarks(string remarks, Guid updatedBy)
    {
        EnsureEditable();

        VisitRemarks = remarks?.Trim();
        Touch(updatedBy);
    }

    public void LinkPhoto(Guid fileId, VisitPhotoCategory category, string? caption, DateTime capturedAtUtc, Guid createdBy)
    {
        EnsureEditable();

        if (category == VisitPhotoCategory.Selfie && HasSelfie)
            throw new InvalidOperationException("Selfie already linked. Use replace flow in a future phase.");

        _photos.Add(VisitPhoto.Create(Id, fileId, category, caption, capturedAtUtc, createdBy));
        Touch(createdBy);
    }

    public void LinkSelfie(Guid fileId, DateTime capturedAtUtc, Guid createdBy)
    {
        EnsureEditable();

        if (HasSelfie)
            throw new InvalidOperationException("Selfie already linked.");

        _photos.Add(VisitPhoto.Create(Id, fileId, VisitPhotoCategory.Selfie, null, capturedAtUtc, createdBy));
        Touch(createdBy);
    }

    public void CaptureLocation(decimal latitude, decimal longitude, DateTime capturedAtUtc, Guid createdBy)
    {
        EnsureEditable();

        if (Location is not null)
            throw new InvalidOperationException("Location already captured.");

        Location = VisitLocation.Create(Id, latitude, longitude, capturedAtUtc, createdBy);
        Touch(createdBy);
    }

    public void LinkSignature(Guid fileId, string signerName, DateTime capturedAtUtc, Guid createdBy)
    {
        EnsureEditable();

        if (Signature is not null)
            throw new InvalidOperationException("Signature already linked.");

        Signature = VisitSignature.Create(Id, fileId, signerName, capturedAtUtc, createdBy);
        Touch(createdBy);
    }

    public void LinkAttachment(Guid fileId, VisitAttachmentType attachmentType, string? title, Guid createdBy)
    {
        EnsureEditable();
        _attachments.Add(VisitAttachment.Create(Id, fileId, attachmentType, title, createdBy));
        Touch(createdBy);
    }

    public void AttachReportPdf(Guid fileId, Guid attachedBy)
    {
        if (ReportStatus != VisitReportStatus.Approved)
            throw new InvalidOperationException("Report PDF can only be attached to approved visits.");

        if (_attachments.Any(a => a.AttachmentType == VisitAttachmentType.ReportPdf))
            return;

        _attachments.Add(VisitAttachment.Create(Id, fileId, VisitAttachmentType.ReportPdf, "Visit Report", attachedBy));
        Touch(attachedBy);
    }

    public void SubmitReport(Guid submittedBy)
    {
        EnsureEditable();
        ValidateSubmissionRequirements();

        ReportStatus = VisitReportStatus.Submitted;
        CompletedAtUtc = DateTime.UtcNow;
        _approvals.Add(VisitApproval.CreatePending(Id, submittedBy));
        Touch(submittedBy);

        RaiseDomainEvent(new VisitReportSubmittedDomainEvent(
            Id.Value, ServiceScheduleId.Value, EngineerId, submittedBy));
    }

    public void Approve(Guid reviewedBy, string? reviewRemarks, Guid siteId)
    {
        if (ReportStatus is not VisitReportStatus.Submitted)
            throw new InvalidOperationException("Only submitted reports can be approved.");

        var pending = _approvals.LastOrDefault(a => a.Decision == VisitApprovalDecision.Pending);
        pending?.Approve(reviewedBy, reviewRemarks);

        ReportStatus = VisitReportStatus.Approved;
        IsCustomerVisible = true;
        Touch(reviewedBy);

        RaiseDomainEvent(new VisitReportApprovedDomainEvent(Id.Value, ServiceScheduleId.Value, reviewedBy));
        RaiseDomainEvent(new VisitCompletedDomainEvent(Id.Value, ServiceScheduleId.Value, siteId));
    }

    public void ReturnForRework(Guid reviewedBy, string returnReason)
    {
        if (ReportStatus is not VisitReportStatus.Submitted)
            throw new InvalidOperationException("Only submitted reports can be returned.");

        if (string.IsNullOrWhiteSpace(returnReason))
            throw new InvalidOperationException("Return reason is required.");

        var pending = _approvals.LastOrDefault(a => a.Decision == VisitApprovalDecision.Pending);
        pending?.Return(reviewedBy, returnReason);

        ReportStatus = VisitReportStatus.Returned;
        IsCustomerVisible = false;
        Touch(reviewedBy);

        RaiseDomainEvent(new VisitReportReturnedDomainEvent(
            Id.Value, ServiceScheduleId.Value, reviewedBy, returnReason.Trim()));
    }

    public void ValidateSubmissionRequirements()
    {
        if (!HasSelfie)
            throw new InvalidOperationException("Selfie photo is required before submitting the visit report.");

        if (!HasBeforeDuringAfterPhoto)
            throw new InvalidOperationException("At least one Before, During, or After photo is required.");

        if (!HasGps)
            throw new InvalidOperationException("GPS location is required before submitting the visit report.");

        if (!HasSignature)
            throw new InvalidOperationException("Customer signature is required before submitting the visit report.");

        if (!HasMinimumRemarks)
            throw new InvalidOperationException($"Visit remarks must be at least {MinRemarksLength} characters.");
    }

    private void EnsureEditable()
    {
        if (ReportStatus is VisitReportStatus.Submitted or VisitReportStatus.Approved)
            throw new InvalidOperationException($"Visit in status {ReportStatus} cannot be modified.");
    }

    private void Touch(Guid updatedBy)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        RowVersion++;
    }
}

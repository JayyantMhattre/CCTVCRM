using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Events;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

/// <summary>Ticket aggregate root (schema <c>cctv_ticket.tickets</c>).</summary>
public sealed class Ticket : AggregateRoot<TicketId>
{
    public const int MaxAttachments = 5;
    public const int MinReopenReasonLength = 10;

    private readonly List<TicketComment> _comments = [];
    private readonly List<TicketAttachment> _attachments = [];
    private readonly List<TicketAssignment> _assignments = [];
    private readonly List<TicketStatusHistory> _statusHistory = [];

    private Ticket(TicketId id) : base(id) { }

    public string TicketNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid SiteId { get; private set; }
    public Guid? AmcContractId { get; private set; }
    public Guid? OriginServiceVisitId { get; private set; }
    public TicketSource Source { get; private set; }
    public string Subject { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTime? ResolvedAtUtc { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public int ReopenCount { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public uint RowVersion { get; private set; }

    public IReadOnlyList<TicketComment> Comments => _comments.AsReadOnly();
    public IReadOnlyList<TicketAttachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyList<TicketAssignment> Assignments => _assignments.AsReadOnly();
    public IReadOnlyList<TicketStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    public TicketAssignment? ActiveAssignment =>
        _assignments.FirstOrDefault(a => a.IsActive);

    public int ActiveAttachmentCount =>
        _attachments.Count(a => !a.IsDeleted);

    public static Ticket Create(
        TicketId id,
        string ticketNumber,
        Guid customerId,
        Guid siteId,
        Guid? amcContractId,
        Guid? originServiceVisitId,
        TicketSource source,
        string subject,
        string description,
        TicketPriority priority,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber))
            throw new InvalidOperationException("Ticket number is required.");

        if (string.IsNullOrWhiteSpace(subject))
            throw new InvalidOperationException("Subject is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidOperationException("Description is required.");

        var ticket = new Ticket(id)
        {
            TicketNumber = ticketNumber.Trim(),
            CustomerId = customerId,
            SiteId = siteId,
            AmcContractId = amcContractId,
            OriginServiceVisitId = originServiceVisitId,
            Source = source,
            Subject = subject.Trim(),
            Description = description.Trim(),
            Priority = priority,
            Status = TicketStatus.Open,
            ReopenCount = 0,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        ticket.RecordStatusChange(null, TicketStatus.Open, null, createdBy);

        ticket.RaiseDomainEvent(new TicketCreatedDomainEvent(
            id.Value,
            ticket.TicketNumber,
            customerId,
            siteId,
            source.ToString(),
            createdBy));

        return ticket;
    }

    public void AssignEngineer(TicketAssignmentId assignmentId, Guid engineerId, Guid assignedBy)
    {
        EnsureNotDeleted();

        if (Status is not TicketStatus.Open and not TicketStatus.Reopened)
            throw new InvalidOperationException($"Ticket in status {Status} cannot be assigned.");

        var isReassignment = ActiveAssignment is not null;
        foreach (var active in _assignments.Where(a => a.IsActive))
            active.Deactivate();

        _assignments.Add(TicketAssignment.Create(assignmentId, Id, engineerId, assignedBy));
        ChangeStatus(TicketStatus.Assigned, assignedBy, null);

        RaiseDomainEvent(new TicketAssignedDomainEvent(
            Id.Value, engineerId, assignedBy, isReassignment));
    }

    public void UpdateStatus(TicketStatus toStatus, TicketAuthorRole actorRole, Guid changedBy, string? reason = null)
    {
        EnsureNotDeleted();

        ValidateStatusTransition(Status, toStatus, actorRole);

        if (toStatus == TicketStatus.Resolved)
            ResolvedAtUtc = DateTime.UtcNow;

        ChangeStatus(toStatus, changedBy, reason);
    }

    public void AddComment(
        TicketCommentId commentId,
        string comment,
        TicketAuthorRole authorRole,
        Guid createdBy)
    {
        EnsureNotDeleted();
        _comments.Add(TicketComment.Create(commentId, Id, comment, authorRole, createdBy));
        Touch(createdBy);
    }

    public TicketAttachment LinkAttachment(
        TicketAttachmentId attachmentId,
        Guid fileId,
        string? title,
        Guid createdBy)
    {
        EnsureNotDeleted();

        if (ActiveAttachmentCount >= MaxAttachments)
            throw new InvalidOperationException($"A ticket cannot have more than {MaxAttachments} attachments.");

        var attachment = TicketAttachment.Create(attachmentId, Id, fileId, title, createdBy);
        _attachments.Add(attachment);
        Touch(createdBy);
        return attachment;
    }

    public void RemoveAttachment(TicketAttachmentId attachmentId, Guid removedBy)
    {
        EnsureNotDeleted();

        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId && !a.IsDeleted)
            ?? throw new InvalidOperationException("Attachment not found.");

        attachment.SoftDelete(removedBy);
        Touch(removedBy);
    }

    public void Close(Guid closedBy)
    {
        EnsureNotDeleted();

        if (Status != TicketStatus.Resolved)
            throw new InvalidOperationException("Only resolved tickets can be closed.");

        ClosedAtUtc = DateTime.UtcNow;
        ChangeStatus(TicketStatus.Closed, closedBy, null);

        RaiseDomainEvent(new TicketClosedDomainEvent(Id.Value, closedBy));
    }

    public void Reopen(string reason, Guid reopenedBy)
    {
        EnsureNotDeleted();

        if (Status != TicketStatus.Closed)
            throw new InvalidOperationException("Only closed tickets can be reopened.");

        if (string.IsNullOrWhiteSpace(reason) || reason.Trim().Length < MinReopenReasonLength)
            throw new InvalidOperationException($"Reopen reason must be at least {MinReopenReasonLength} characters.");

        ReopenCount++;
        ResolvedAtUtc = null;
        ClosedAtUtc = null;
        ChangeStatus(TicketStatus.Reopened, reopenedBy, reason.Trim());

        RaiseDomainEvent(new TicketReopenedDomainEvent(
            Id.Value, reason.Trim(), ReopenCount, reopenedBy));
    }

    private void ChangeStatus(TicketStatus toStatus, Guid changedBy, string? reason)
    {
        var fromStatus = Status;
        Status = toStatus;
        RecordStatusChange(fromStatus, toStatus, reason, changedBy);
        Touch(changedBy);

        RaiseDomainEvent(new TicketStatusChangedDomainEvent(
            Id.Value,
            fromStatus.ToString(),
            toStatus.ToString(),
            changedBy,
            reason));
    }

    private void RecordStatusChange(
        TicketStatus? fromStatus,
        TicketStatus toStatus,
        string? reason,
        Guid changedBy)
    {
        _statusHistory.Add(TicketStatusHistory.Create(
            TicketStatusHistoryId.New(),
            Id,
            fromStatus,
            toStatus,
            reason,
            changedBy));
    }

    private static void ValidateStatusTransition(
        TicketStatus fromStatus,
        TicketStatus toStatus,
        TicketAuthorRole actorRole)
    {
        var allowed = (fromStatus, toStatus, actorRole) switch
        {
            (TicketStatus.Assigned, TicketStatus.InProgress, TicketAuthorRole.Engineer) => true,
            (TicketStatus.Assigned, TicketStatus.InProgress, TicketAuthorRole.Admin) => true,
            (TicketStatus.InProgress, TicketStatus.Resolved, TicketAuthorRole.Engineer) => true,
            (TicketStatus.InProgress, TicketStatus.Resolved, TicketAuthorRole.Admin) => true,
            _ => false
        };

        if (!allowed)
            throw new InvalidOperationException(
                $"Transition from {fromStatus} to {toStatus} is not allowed for role {actorRole}.");
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Ticket has been deleted.");
    }

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }
}

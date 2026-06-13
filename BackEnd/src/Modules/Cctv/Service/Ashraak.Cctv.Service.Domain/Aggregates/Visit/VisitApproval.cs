using Ashraak.Cctv.Service.Domain.Enums;

namespace Ashraak.Cctv.Service.Domain.Aggregates.Visit;

public sealed class VisitApproval
{
    private VisitApproval() { }

    public Guid Id { get; private set; }
    public ServiceVisitId ServiceVisitId { get; private set; }
    public VisitApprovalDecision Decision { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string? ReviewRemarks { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    public static VisitApproval CreatePending(ServiceVisitId visitId, Guid createdBy)
    {
        return new VisitApproval
        {
            Id = Guid.NewGuid(),
            ServiceVisitId = visitId,
            Decision = VisitApprovalDecision.Pending,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Approve(Guid reviewedBy, string? reviewRemarks)
    {
        Decision = VisitApprovalDecision.Approved;
        ReviewedBy = reviewedBy;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewRemarks = reviewRemarks?.Trim();
    }

    public void Return(Guid reviewedBy, string returnReason)
    {
        if (string.IsNullOrWhiteSpace(returnReason))
            throw new InvalidOperationException("Return reason is required.");

        Decision = VisitApprovalDecision.Returned;
        ReviewedBy = reviewedBy;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewRemarks = returnReason.Trim();
    }
}

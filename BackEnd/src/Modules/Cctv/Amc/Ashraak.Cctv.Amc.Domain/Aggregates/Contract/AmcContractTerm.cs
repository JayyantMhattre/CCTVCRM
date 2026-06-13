using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Amc.Domain.Aggregates.Contract;

/// <summary>AMC contract term (composition under <see cref="AmcContract"/>).</summary>
public sealed class AmcContractTerm : Entity<AmcContractTermId>
{
    private AmcContractTerm(AmcContractTermId id) : base(id) { }

    public AmcContractId ContractId { get; private set; }
    public int TermNo { get; private set; }
    public AmcPlanVersionId PlanVersionId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public decimal AgreedPrice { get; private set; }
    public TermStatus Status { get; private set; }
    public TermOrigin Origin { get; private set; }
    public bool RenewalRequestedByCustomer { get; private set; }
    public DateTime? RenewalRequestedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public uint RowVersion { get; private set; }

    internal static AmcContractTerm Create(
        AmcContractTermId id,
        AmcContractId contractId,
        int termNo,
        AmcPlanVersionId planVersionId,
        DateOnly startDate,
        DateOnly endDate,
        decimal agreedPrice,
        TermOrigin origin,
        Guid createdBy)
    {
        ValidateDates(startDate, endDate);
        ValidatePrice(agreedPrice);

        return new AmcContractTerm(id)
        {
            ContractId = contractId,
            TermNo = termNo,
            PlanVersionId = planVersionId,
            StartDate = startDate,
            EndDate = endDate,
            AgreedPrice = agreedPrice,
            Status = TermStatus.Draft,
            Origin = origin,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };
    }

    internal void Activate(Guid activatedBy)
    {
        if (Status != TermStatus.Draft)
            throw new InvalidOperationException("Only draft terms can be activated.");

        Status = TermStatus.Active;
        Touch(activatedBy);
    }

    internal void DeactivateToExpired(Guid updatedBy)
    {
        if (Status != TermStatus.Active)
            return;

        Status = TermStatus.Expired;
        Touch(updatedBy);
    }

    internal void Cancel(Guid cancelledBy)
    {
        if (Status is TermStatus.Expired or TermStatus.Cancelled)
            return;

        Status = TermStatus.Cancelled;
        Touch(cancelledBy);
    }

    internal void RequestRenewal(Guid requestedBy)
    {
        if (Status != TermStatus.Active)
            throw new InvalidOperationException("Renewal can only be requested for the active term.");

        RenewalRequestedByCustomer = true;
        RenewalRequestedAtUtc = DateTime.UtcNow;
        Touch(requestedBy);
    }

    internal static void ValidateDates(DateOnly startDate, DateOnly endDate)
    {
        if (endDate <= startDate)
            throw new InvalidOperationException("Term end date must be after start date.");
    }

    internal static void ValidatePrice(decimal price)
    {
        if (price <= 0)
            throw new InvalidOperationException("Term price must be greater than zero.");
    }

    private void Touch(Guid userId)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        RowVersion++;
    }
}

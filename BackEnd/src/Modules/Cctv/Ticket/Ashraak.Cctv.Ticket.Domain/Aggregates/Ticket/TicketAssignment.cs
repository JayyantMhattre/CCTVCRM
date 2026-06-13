using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;

/// <summary>Engineer assignment history (one active per ticket).</summary>
public sealed class TicketAssignment : Entity<TicketAssignmentId>
{
    private TicketAssignment(TicketAssignmentId id) : base(id) { }

    public TicketId TicketId { get; private set; }
    public Guid EngineerId { get; private set; }
    public Guid AssignedBy { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }

    internal static TicketAssignment Create(
        TicketAssignmentId id,
        TicketId ticketId,
        Guid engineerId,
        Guid assignedBy)
    {
        return new TicketAssignment(id)
        {
            TicketId = ticketId,
            EngineerId = engineerId,
            AssignedBy = assignedBy,
            AssignedAtUtc = DateTime.UtcNow,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = assignedBy
        };
    }

    internal void Deactivate()
    {
        IsActive = false;
    }
}

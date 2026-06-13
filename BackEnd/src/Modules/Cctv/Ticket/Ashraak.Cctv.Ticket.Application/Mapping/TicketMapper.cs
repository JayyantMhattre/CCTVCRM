using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Enums;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Application.Mapping;

internal static class TicketMapper
{
    public static TicketSummaryDto ToSummary(TicketAggregate ticket) =>
        new(
            ticket.Id.Value,
            ticket.TicketNumber,
            ticket.CustomerId,
            ticket.SiteId,
            ticket.Subject,
            ToPriority(ticket.Priority),
            ToStatus(ticket.Status),
            ticket.ActiveAssignment?.EngineerId,
            ticket.CreatedAtUtc,
            ticket.ResolvedAtUtc,
            ticket.ClosedAtUtc,
            ticket.ReopenCount);

    public static TicketDetailDto ToDetail(TicketAggregate ticket) =>
        new(
            ticket.Id.Value,
            ticket.TicketNumber,
            ticket.CustomerId,
            ticket.SiteId,
            ticket.AmcContractId,
            ticket.OriginServiceVisitId,
            ToSource(ticket.Source),
            ticket.Subject,
            ticket.Description,
            ToPriority(ticket.Priority),
            ToStatus(ticket.Status),
            ticket.ResolvedAtUtc,
            ticket.ClosedAtUtc,
            ticket.ReopenCount,
            ticket.CreatedAtUtc,
            ticket.CreatedBy,
            ticket.UpdatedAtUtc,
            ticket.UpdatedBy,
            ticket.RowVersion,
            ticket.ActiveAssignment?.EngineerId,
            ticket.Comments.Select(ToComment).ToList(),
            ticket.Attachments.Where(a => !a.IsDeleted).Select(ToAttachment).ToList(),
            ticket.StatusHistory.Select(ToStatusHistory).ToList());

    public static TicketCommentDto ToComment(TicketComment comment) =>
        new(
            comment.Id.Value,
            comment.Comment,
            ToAuthorRole(comment.AuthorRole),
            comment.CreatedAtUtc,
            comment.CreatedBy);

    public static TicketAttachmentDto ToAttachment(TicketAttachment attachment) =>
        new(
            attachment.Id.Value,
            attachment.FileId,
            attachment.Title,
            attachment.CreatedAtUtc,
            attachment.CreatedBy);

    public static TicketStatusHistoryDto ToStatusHistory(TicketStatusHistory history) =>
        new(
            history.Id.Value,
            history.FromStatus.HasValue ? ToStatus(history.FromStatus.Value) : null,
            ToStatus(history.ToStatus),
            history.Reason,
            history.ChangedAtUtc,
            history.ChangedBy);

    public static string ToStatus(TicketStatus status) => status switch
    {
        TicketStatus.Open => TicketStatusContract.Open,
        TicketStatus.Assigned => TicketStatusContract.Assigned,
        TicketStatus.InProgress => TicketStatusContract.InProgress,
        TicketStatus.Resolved => TicketStatusContract.Resolved,
        TicketStatus.Closed => TicketStatusContract.Closed,
        TicketStatus.Reopened => TicketStatusContract.Reopened,
        _ => status.ToString()
    };

    public static string ToPriority(TicketPriority priority) => priority switch
    {
        TicketPriority.Low => TicketPriorityContract.Low,
        TicketPriority.Medium => TicketPriorityContract.Medium,
        TicketPriority.High => TicketPriorityContract.High,
        TicketPriority.Critical => TicketPriorityContract.Critical,
        _ => priority.ToString()
    };

    public static string ToSource(TicketSource source) => source switch
    {
        TicketSource.Customer => TicketSourceContract.Customer,
        TicketSource.Admin => TicketSourceContract.Admin,
        TicketSource.EngineerVisit => TicketSourceContract.EngineerVisit,
        _ => source.ToString()
    };

    public static string ToAuthorRole(TicketAuthorRole role) => role switch
    {
        TicketAuthorRole.Customer => TicketAuthorRoleContract.Customer,
        TicketAuthorRole.Admin => TicketAuthorRoleContract.Admin,
        TicketAuthorRole.Engineer => TicketAuthorRoleContract.Engineer,
        _ => role.ToString()
    };

    public static bool TryParseStatus(string value, out TicketStatus status)
    {
        status = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Trim() switch
        {
            TicketStatusContract.Open => Assign(TicketStatus.Open, out status),
            TicketStatusContract.Assigned => Assign(TicketStatus.Assigned, out status),
            TicketStatusContract.InProgress => Assign(TicketStatus.InProgress, out status),
            TicketStatusContract.Resolved => Assign(TicketStatus.Resolved, out status),
            TicketStatusContract.Closed => Assign(TicketStatus.Closed, out status),
            TicketStatusContract.Reopened => Assign(TicketStatus.Reopened, out status),
            _ => false
        };
    }

    public static bool TryParsePriority(string value, out TicketPriority priority)
    {
        priority = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Trim() switch
        {
            TicketPriorityContract.Low => Assign(TicketPriority.Low, out priority),
            TicketPriorityContract.Medium => Assign(TicketPriority.Medium, out priority),
            TicketPriorityContract.High => Assign(TicketPriority.High, out priority),
            TicketPriorityContract.Critical => Assign(TicketPriority.Critical, out priority),
            _ => false
        };
    }

    public static TicketAuthorRole ResolveAuthorRole(
        bool isAdmin,
        bool isEngineer,
        bool isCustomer)
    {
        if (isAdmin)
            return TicketAuthorRole.Admin;

        if (isEngineer)
            return TicketAuthorRole.Engineer;

        if (isCustomer)
            return TicketAuthorRole.Customer;

        return TicketAuthorRole.Admin;
    }

    private static bool Assign<T>(T value, out T output)
    {
        output = value;
        return true;
    }
}

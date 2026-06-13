using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Enums;
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;

namespace Ashraak.Cctv.Invoice.Application.Mapping;

internal static class InvoiceMapper
{
    public static InvoiceSummaryDto ToSummary(InvoiceAggregate invoice) =>
        new(
            invoice.Id.Value,
            invoice.InvoiceNumber,
            invoice.CustomerId,
            invoice.SiteId,
            ToType(invoice.InvoiceType),
            ToStatus(invoice.Status),
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.TotalAmount,
            invoice.CreatedAtUtc,
            invoice.PaidAtUtc);

    public static InvoiceDetailDto ToDetail(InvoiceAggregate invoice) =>
        new(
            invoice.Id.Value,
            invoice.InvoiceNumber,
            invoice.CustomerId,
            invoice.SiteId,
            ToType(invoice.InvoiceType),
            invoice.AmcContractTermId,
            invoice.TicketId,
            invoice.ServiceVisitId,
            invoice.InvoiceDate,
            invoice.DueDate,
            invoice.SubtotalAmount,
            invoice.TaxAmount,
            invoice.TotalAmount,
            ToStatus(invoice.Status),
            invoice.PaidAtUtc,
            invoice.CreatedAtUtc,
            invoice.CreatedBy,
            invoice.UpdatedAtUtc,
            invoice.UpdatedBy,
            invoice.RowVersion,
            invoice.Lines.Select(ToLine).ToList(),
            invoice.Attachments.Select(ToAttachment).ToList(),
            invoice.StatusHistory.Select(ToStatusHistory).ToList());

    public static InvoiceLineDto ToLine(InvoiceLine line) =>
        new(
            line.Id.Value,
            line.LineNo,
            line.Description,
            line.Quantity,
            line.UnitPrice,
            line.LineTotal);

    public static InvoiceAttachmentDto ToAttachment(InvoiceAttachment attachment) =>
        new(
            attachment.Id.Value,
            attachment.FileId,
            ToAttachmentType(attachment.AttachmentType),
            attachment.CreatedAtUtc,
            attachment.CreatedBy);

    public static InvoiceStatusHistoryDto ToStatusHistory(InvoiceStatusHistory history) =>
        new(
            history.Id.Value,
            history.FromStatus.HasValue ? ToStatus(history.FromStatus.Value) : null,
            ToStatus(history.ToStatus),
            history.ChangedAtUtc,
            history.ChangedBy);

    public static IReadOnlyList<InvoiceAggregate.LineDraft> ToLineDrafts(IReadOnlyList<InvoiceLineRequest> lines) =>
        lines.Select(l => new InvoiceAggregate.LineDraft(
            l.Description,
            l.Quantity,
            l.UnitPrice,
            l.SortOrder)).ToList();

    public static decimal ResolveTaxAmount(IReadOnlyList<InvoiceLineRequest> lines, decimal? explicitTax)
    {
        if (explicitTax.HasValue)
            return Math.Round(explicitTax.Value, 2, MidpointRounding.AwayFromZero);

        decimal tax = 0;
        foreach (var line in lines)
        {
            if (!line.TaxRate.HasValue || line.TaxRate.Value <= 0)
                continue;

            var lineTotal = InvoiceLine.ComputeLineTotal(line.Quantity, line.UnitPrice);
            tax += Math.Round(lineTotal * line.TaxRate.Value / 100m, 2, MidpointRounding.AwayFromZero);
        }

        return Math.Round(tax, 2, MidpointRounding.AwayFromZero);
    }

    public static string ToStatus(InvoiceStatus status) => status switch
    {
        InvoiceStatus.Draft => InvoiceStatusContract.Draft,
        InvoiceStatus.Generated => InvoiceStatusContract.Generated,
        InvoiceStatus.Sent => InvoiceStatusContract.Sent,
        InvoiceStatus.Paid => InvoiceStatusContract.Paid,
        InvoiceStatus.Cancelled => InvoiceStatusContract.Cancelled,
        _ => status.ToString()
    };

    public static string ToType(InvoiceType type) => type switch
    {
        InvoiceType.AmcRenewal => InvoiceTypeContract.AmcRenewal,
        InvoiceType.NewAmc => InvoiceTypeContract.NewAmc,
        InvoiceType.EmergencyService => InvoiceTypeContract.EmergencyService,
        InvoiceType.SpareReplacement => InvoiceTypeContract.SpareReplacement,
        InvoiceType.AdditionalCharges => InvoiceTypeContract.AdditionalCharges,
        InvoiceType.Other => InvoiceTypeContract.Other,
        _ => type.ToString()
    };

    public static string ToAttachmentType(InvoiceAttachmentType type) => type switch
    {
        InvoiceAttachmentType.InvoicePdf => InvoiceAttachmentTypeContract.InvoicePdf,
        InvoiceAttachmentType.Supporting => InvoiceAttachmentTypeContract.Supporting,
        _ => type.ToString()
    };

    public static bool TryParseStatus(string value, out InvoiceStatus status)
    {
        status = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Trim() switch
        {
            InvoiceStatusContract.Draft => Assign(InvoiceStatus.Draft, out status),
            InvoiceStatusContract.Generated => Assign(InvoiceStatus.Generated, out status),
            InvoiceStatusContract.Sent => Assign(InvoiceStatus.Sent, out status),
            InvoiceStatusContract.Paid => Assign(InvoiceStatus.Paid, out status),
            InvoiceStatusContract.Cancelled => Assign(InvoiceStatus.Cancelled, out status),
            _ => false
        };
    }

    public static bool TryParseType(string value, out InvoiceType type)
    {
        type = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return value.Trim() switch
        {
            InvoiceTypeContract.AmcRenewal => Assign(InvoiceType.AmcRenewal, out type),
            InvoiceTypeContract.NewAmc => Assign(InvoiceType.NewAmc, out type),
            InvoiceTypeContract.EmergencyService => Assign(InvoiceType.EmergencyService, out type),
            InvoiceTypeContract.SpareReplacement => Assign(InvoiceType.SpareReplacement, out type),
            InvoiceTypeContract.AdditionalCharges => Assign(InvoiceType.AdditionalCharges, out type),
            InvoiceTypeContract.Other => Assign(InvoiceType.Other, out type),
            _ => false
        };
    }

    public static bool RequiresAmcTerm(InvoiceType type) =>
        type is InvoiceType.AmcRenewal or InvoiceType.NewAmc;

    private static bool Assign<T>(T value, out T output)
    {
        output = value;
        return true;
    }
}
